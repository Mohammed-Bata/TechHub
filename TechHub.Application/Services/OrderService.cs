using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Product = TechHub.Domain.Entities.Product;
using TechHub.Domain.Entities;

namespace TechHub.Application.Services
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly ICacheService _cache;
        private readonly IEmailService _emailService;
        private readonly APIResponse _response = new APIResponse();

        public OrderService(IUnitOfWork unitOfWork, IPaymentService paymentService, ICacheService cache, IEmailService emailService)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _emailService = emailService;
        }

        public async Task<Order> GetOrder(Guid id,string userId)
        {
            var cacheKey = $"Order_{id}_{userId}";
            var cachedOrder = await _cache.GetAsync<Order>(cacheKey);
            if (cachedOrder != null)
            {
                return cachedOrder;
            }

            var order = await _unitOfWork.Orders.GetAsync(o => o.Id == id && o.UserId == userId, includes: o => o.Include(o => o.OrderItems));
            await _cache.SetAsync(cacheKey, order);

            return order;
        }

        public async Task<List<Order>> GetOrders(string userId)
        {
            string cacheKey = $"Orders_{userId}";
            var cachedOrders = await _cache.GetAsync<List<Order>>(cacheKey);
            if (cachedOrders != null)
            {
                return cachedOrders;
            }

            var orders = await _unitOfWork.Orders.GetAll(o => o.UserId == userId, includes: o => o.Include(o => o.OrderItems));
            await _cache.SetAsync(cacheKey, orders);

            return orders.ToList();
        }

       
        public async Task<APIResponse> CreateOrder(OrderDto orderDto,string userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Cart is empty." };
                return _response;
            }

            var paymentIntent = await _paymentService.CreateOrUpdatePaymentIntent(userId);
            if (paymentIntent == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Payment processing failed." };
                return _response;
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.Price,
                AddressId = orderDto.AddressId,
                PaymentIntentId = paymentIntent.Id,
                OrderItems = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                }).ToList()
            };

            var productIds = order.OrderItems.Select(p => p.ProductId).ToList();
            List<Product> Products = new List<Product>();
            foreach (var productId in productIds)
            {
                var product = await _unitOfWork.Products.GetAsync(p => p.Id == productId);
                Products.Add(product);
            }

            foreach (var orderItem in order.OrderItems)
            {
                var product = Products.FirstOrDefault(p => p.Id == orderItem.ProductId);
                if (product == null || product.StockAmount < orderItem.Quantity)
                {
                    _response.Errors = new List<string> { "Product out of stock." };
                    return _response;
                }
                product.StockAmount -= orderItem.Quantity;
                await _unitOfWork.Products.UpdateAsync(product);
            }
            cart.Items.Clear();
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            await _emailService.SendEmail(user.Email, "Order", "You Made Order");

            _response.Data = order;

            string cacheKey = $"Orders_{userId}";
            await _cache.RemoveAsync(cacheKey);

            return _response;
        }
    }
}
