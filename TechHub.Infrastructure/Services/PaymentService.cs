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
        public PaymentService(IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(decimal cartPrice)
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cartPrice * 100,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var paymentIntent = await paymentIntentService.CreateAsync(options);
            return paymentIntent;
        }
    }
}
