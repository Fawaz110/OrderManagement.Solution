using Core.Entities.Order.Aggregate;
using Core.Services.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Entities;
using OrderManagement.Errors;
using Service.Dtos;

namespace OrderManagement.Controllers
{
    public class CustomersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        public CustomersController(
            UserManager<AppUser> userManager,
            IAuthService authService,
            IOrderService orderService)
        {
            _userManager = userManager;
            _authService = authService;
            _orderService = orderService;
        }

        [HttpPost] // POST: /api/customers
        public async Task<ActionResult<UserDto>> AddCustomer([FromBody] RegisterDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
                return BadRequest(new ApiResponse(400, "This User Already Existed"));

            user = new AppUser
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Name + Guid.NewGuid().ToString().Split('-')[0]
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            result = await _userManager.AddToRoleAsync(user, "Customer");

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            return Ok(new UserDto
            {
                Email = user.Email,
                Name = model.Name,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }


        [HttpGet("{customerId}/orders")]// GET: /api/customers/{customerId}/orders
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(string customerId)
        {
            var user = await _userManager.FindByIdAsync(customerId);

            if (user is null)
                return BadRequest(new ApiResponse(400));

            var orders = await _orderService.GetAllOrdersForUserAsync(customerId);

            if (!orders.Any())
                return NotFound(new ApiResponse(404));

            return Ok(orders);
        }
    }
}
