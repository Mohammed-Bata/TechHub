using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechHub.Application.Common;
using TechHub.Application.Common.Interfaces;
using TechHub.Application.Interfaces;
using TechHub.Infrastructure.Services;

namespace TechHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddPersistence(services, configuration);
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IImageService, ImageService>();

            // Map both interfaces to the same implementation
            services.AddScoped<IAppDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());

          
            return services;
        }

        private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Database"))
                .EnableSensitiveDataLogging()
                 .LogTo(Console.WriteLine, LogLevel.Information)
                );

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<DbInitializer>();

        }

      
    }
}
