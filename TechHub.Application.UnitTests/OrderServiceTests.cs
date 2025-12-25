using Microsoft.AspNetCore.Http;
using NSubstitute;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Services;
using TechHub.Domain.Entities;
using Product = TechHub.Domain.Entities.Product;

namespace TechHub.Application.UnitTests
{
    public class OrderServiceTests
    {
        private readonly IUnitOfWork _unitOfWorkMock = NSubstitute.Substitute.For<IUnitOfWork>();
        private readonly IPaymentService _paymentServiceMock = NSubstitute.Substitute.For<IPaymentService>();
        private readonly ICacheService _cacheMock = NSubstitute.Substitute.For<ICacheService>();
        private readonly IEmailService _emailServiceMock = NSubstitute.Substitute.For<IEmailService>();
        private readonly IRepository<Order> _OrderRepositoryMock = NSubstitute.Substitute.For<IRepository<Order>>();
        private readonly ICartRepository _cartRepositoryMock = NSubstitute.Substitute.For<ICartRepository>();
        private readonly OrderService _orderService;
        public OrderServiceTests()
        {
            _unitOfWorkMock.Orders.Returns(_OrderRepositoryMock);
            _unitOfWorkMock.Carts.Returns(_cartRepositoryMock);
            _orderService = new OrderService(_unitOfWorkMock, _paymentServiceMock, _cacheMock, _emailServiceMock);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnCachedOrder_WhenCacheExists()
        {
            // Arrange
            var userId = "testUserId";
            var orderId = 1;
            var cachedOrder = new Order { Id = orderId, UserId = userId };
            _cacheMock.GetAsync<Order>($"Order_{orderId}_{userId}").Returns(cachedOrder);
            // Act
            var result = await _orderService.GetOrder(orderId, userId);
            // Assert
            Assert.Equal(cachedOrder, result);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnOrderFromRepository_WhenCacheDoesNotExist()
        {
            // Arrange
            var userId = "testUserId";
            var orderId = 1;
            var order = new Order { Id = orderId, UserId = userId };
            _cacheMock.GetAsync<Order>($"Order_{orderId}_{userId}").Returns((Order)null);
            _OrderRepositoryMock.GetAsync(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Func<IQueryable<Order>, IQueryable<Order>>>())
                .Returns(order);
            // Act
            var result = await _orderService.GetOrder(orderId, userId);
            // Assert
            Assert.Equal(order, result);

        }

        [Fact]
        public async Task GetOrders_ShouldReturnCachedOrders_WhenCacheExists()
        {
            // Arrange
            var userId = "testUserId";
            var cachedOrders = new List<Order>
            {
                new Order { Id = 1, UserId = userId },
                new Order { Id = 2, UserId = userId }
            };
            _cacheMock.GetAsync<List<Order>>($"Orders_{userId}").Returns(cachedOrders);
            // Act
            var result = await _orderService.GetOrders(userId);
            // Assert
            Assert.Equal(cachedOrders, result);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOrdersFromRepository_WhenCacheDoesNotExist()
        {
            // Arrange
            var userId = "testUserId";
            var orders = new List<Order>
            {
                new Order { Id = 1, UserId = userId },
                new Order { Id = 2, UserId = userId }
            };
            _cacheMock.GetAsync<List<Order>>($"Orders_{userId}").Returns((List<Order>)null);
            _OrderRepositoryMock.GetAll(Arg.Any<Expression<Func<Order, bool>>>(), Arg.Any<Func<IQueryable<Order>, IQueryable<Order>>>())
                .Returns(orders.AsQueryable());
            // Act
            var result = await _orderService.GetOrders(userId);
            // Assert
            Assert.Equal(orders, result.ToList());
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnError_WhenCartIsEmpty()
        {
            // Arrange
            var orderDto = new OrderDto(1);
            var userId = "testUserId";

            // Act
            var result = await _orderService.CreateOrder(orderDto, userId);
            // Assert
            Assert.Equal("Cart is empty.", result.Errors.FirstOrDefault());
        }
        [Fact]
        public async Task CreateOrder_ShouldReturnError_WhenPaymentFails()
        {
            // Arrange
            var orderDto = new OrderDto(1);
            var userId = "testUserId";
            var cart = new ShoppingCart { UserId = userId,
                Items = new List<ShoppingCartItem> {
                    new ShoppingCartItem
                    {
                        Product = new Product { Id = 1, Price = 100, Name = "Test Product" },
                        Quantity = 1,
                        Price = 100,
                    }
                } 
            };
            _cartRepositoryMock.GetCartWithItemsAsync(userId).Returns(cart);
            _paymentServiceMock.CreateOrUpdatePaymentIntent(userId).Returns((PaymentIntent)null);

            // Act
            var result = await _orderService.CreateOrder(orderDto, userId);

            // Assert
            Assert.Equal("Payment processing failed.", result.Errors.FirstOrDefault());
        }
    }
}
