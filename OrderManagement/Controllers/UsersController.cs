using Core.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Dtos;
using OrderManagement.Entities;
using OrderManagement.Errors;
using System.Security.Claims;

namespace OrderManagement.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;

        public UsersController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("login")] // POST: /api/users/login
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            return Ok(new UserDto
            {
                Name = user.Name,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
        /// <summary>
        /// end point to register user as an admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("register")] 
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto model)
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

            result = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            return Ok(new UserDto
            {
                Email = user.Email,
                Name = model.Name,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var email = User.FindFirstValue("email");
            var user = await _userManager.FindByEmailAsync(email);

            return Ok(new UserDto
            {
                Email = email,
                Name = user.Name,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
    }
}
