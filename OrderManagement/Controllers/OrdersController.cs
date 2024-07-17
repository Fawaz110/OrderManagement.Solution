using AutoMapper;
using Core.Entities.Order.Aggregate;
using Core.Services.Contract;
using Core.Specifications.OrderSpeicifcations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Entities;
using OrderManagement.Errors;
using Service.Dtos.OrderDtos;

namespace OrderManagement.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public OrdersController(
            IOrderService orderService,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _orderService = orderService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost] // POST: /api/orders
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.buyerEmail);

            if (user is null)
                return BadRequest(new ApiResponse(400));

            var isCustomer = await _userManager.IsInRoleAsync(user, "Customer");

            if (!isCustomer)
                return BadRequest(new ApiResponse(400));

            var items = _mapper.Map<List<OrderItem>>(model.Items);

            var order = await _orderService.CreateOrderAsync(model.buyerEmail, items, model.PaymentMethod, model.OrderStatus);

            if (order is null)
                return BadRequest(new ApiResponse(400, "an error occured during adding order"));

            return Ok(order);
        }

        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet] // GET: /api/orders
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var spec = new OrderWithItemsSpeicifcations();

            var orders = await _orderService.GetAllWithSpecAsync(spec);

            if (!orders.Any())
                return NotFound(new ApiResponse(404));

            return Ok(orders);
        }

        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{orderId}")] // GET: /api/orders/{orderId}
        public async Task<ActionResult<IEnumerable<Order>>> GetById(int orderId)
        {
            var spec = new OrderWithItemsSpeicifcations(orderId);

            var order = await _orderService.GetWithSpecAsync(spec);

            if (order is null)
                return NotFound(new ApiResponse(404));

            return Ok(order);
        }

        [HttpPut("{orderId}/status")] // PUT: /api/orders/{orderId}/status
        public async Task<ActionResult<ApiResponse>> UpdateStatus(int orderId, OrderStatus? status = null)
        {
            if (status == null)
            {
                var error = new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Status field is required" }
                };
                return BadRequest(error);
            }

            var order = await _orderService.UpdateStatusAsync(orderId, (OrderStatus)status);

            if (order is null)
                return BadRequest(new ApiResponse(400));

            return Ok(order);
        }
    }
}
