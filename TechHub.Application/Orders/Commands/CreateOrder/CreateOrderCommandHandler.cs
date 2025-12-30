using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Domain.Entities;

namespace TechHub.Application.Orders.Commands.CreateOrder
{
   public class CreateOrderCommandHandler: IRequestHandler<CreateOrderCommand,Guid>
    {
        private readonly IAppDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;

        public CreateOrderCommandHandler(IAppDbContext context, IPaymentService paymentService, IEmailService emailService)
        {
            _context = context;
            _paymentService = paymentService;
            _emailService = emailService;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (cart == null || !cart.Items.Any())
            {
                throw new Exception("Cart is empty.");
            }

            var paymentIntent = await _paymentService.CreateOrUpdatePaymentIntent(request.UserId);

            if (paymentIntent == null)
            {
                throw new Exception("Failed to create payment intent.");
            }

            var order = new Order
            {
                UserId = request.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.Price,
                AddressId = request.AddressId,
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
                var product = await _context.Products.FindAsync(productId);
                Products.Add(product);
            }

            foreach (var orderItem in order.OrderItems)
            {
                var product = Products.FirstOrDefault(p => p.Id == orderItem.ProductId);
                if (product == null || product.StockAmount < orderItem.Quantity)
                {
                   throw new Exception($"Product with ID {orderItem.ProductId} is out of stock.");
                }
                product.StockAmount -= orderItem.Quantity;
               
            }
            cart.Items.Clear();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(cancellationToken);

            var user = await _context.Users.FindAsync(request.UserId);

            await _emailService.SendEmail(user.Email, "Order", "You Made Order");

            return order.Id;

        }
    }
}
