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
        private readonly IAuthService _authService;

        public CreateOrderCommandHandler(IAppDbContext context, IPaymentService paymentService, IEmailService emailService, IAuthService authService)
        {
            _context = context;
            _paymentService = paymentService;
            _emailService = emailService;
            _authService = authService;
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

            var paymentIntent = await _paymentService.CreateOrUpdatePaymentIntent(cart.Price);

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

            foreach (var item in cart.Items)
            {
                var product = item.Product;
                if (product == null || product.StockAmount < item.StockAmount)
                {
                   throw new Exception($"Product with ID {product.Id} is out of stock.");
                }
                product.StockAmount -= item.Quantity;
               
            }
            
            _context.CartItems.RemoveRange(cart.Items);
           
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(cancellationToken);

          

            //var userEmail = await _authService.FindEmailByIdAsync(request.UserId);

            //fake email
            //await _emailService.SendEmail(userEmail, "Order", "You Made Order");

            return order.Id;

        }
    }
}
