using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.Interfaces;

namespace TechHub.Infrastructure.Services
{
    public class PaymentService :IPaymentService
    {
        private readonly ICartRepository _ShoppingCartRepository;
        public PaymentService(ICartRepository shoppingCartRepository, IConfiguration configuration)
        {
            _ShoppingCartRepository = shoppingCartRepository;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(string userId)
        {
            var cart = await _ShoppingCartRepository.GetCartWithItemsAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                return null;
            }

            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Price * 100,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var paymentIntent = await paymentIntentService.CreateAsync(options);
            return paymentIntent;
        }
    }
}
