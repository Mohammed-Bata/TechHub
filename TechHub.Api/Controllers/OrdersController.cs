using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;
using System.Net;
using System.Security.Claims;
using TechHub.Application.DTOs;
using TechHub.Application.Interfaces;
using TechHub.Application.Services;
using TechHub.Domain;
using TechHub.Infrastructure.Repositories;
using TechHub.Infrastructure.Services;
using Order = TechHub.Domain.Order;
using OrderService = TechHub.Application.Services.OrderService;

namespace TechHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly APIResponse _response;
        private readonly AddressService _addressService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        

        public OrdersController(IPaymentService paymentService,OrderService orderService,
            IUnitOfWork unitOfWork,AddressService addressService)
        {
            _orderService = orderService;
            _addressService = addressService;
            _response = new APIResponse();
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Retrieves an order by its ID for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>An APIResponse containing the order details if found, or an appropriate error response.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetOrder(int id)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.Errors = new List<string> { "User not authorized" };
                return Unauthorized(_response);
            }

            var order = await _orderService.GetOrder(id, userId);
            if (order == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Errors = new List<string> { new string("Order not found.") };

                return NotFound(_response);
            }
            _response.Data = order;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        /// <summary>
        /// Retrieves all orders for the authenticated user.
        /// </summary>
        /// <returns>An APIResponse containing the list of orders if successful, or an appropriate error response.</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.Errors = new List<string> { "User not authorized" };

                return Unauthorized(_response);
            }
            var orders = await _orderService.GetOrders(userId);

            _response.Data = orders;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        /// <summary>
        /// Creates a new order for the authenticated user.
        /// </summary>
        /// <param name="orderDto">The order details to create.</param>
        /// <param name="validator">The validator for the order details.</param>
        /// <returns>An APIResponse containing the created order details if successful, or an appropriate error response.</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> CreateOrder(OrderDto orderDto,
            IValidator<OrderDto> validator)
        {
            var validationResult = await validator.ValidateAsync(orderDto);

            if (!validationResult.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.Errors = new List<string> { "User not authorized" };
                return Unauthorized(_response);
            }

            var address = await _addressService.GetAddress(orderDto.AddressId, userId);
            if (address == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Address not found." };
                return BadRequest(_response);
            }
            var order = await _orderService.CreateOrder(orderDto, userId);
            if (order.StatusCode == HttpStatusCode.BadRequest)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string> { "Order creation failed." };
                return BadRequest(_response);
            }

            _response.Data = order.Data;
            _response.StatusCode = HttpStatusCode.Created;

            return Ok(_response);
        }
    }
}
