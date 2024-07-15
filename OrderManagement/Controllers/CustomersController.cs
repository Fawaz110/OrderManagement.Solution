using Core.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Dtos;
using OrderManagement.Entities;
using OrderManagement.Errors;

namespace OrderManagement.Controllers
{
    public class CustomersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;

        public CustomersController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
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


    }
}
