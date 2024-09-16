using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Service.Interface;
using Common.DTOs;

namespace API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            var token = await _authService.AuthenticateAsync(loginDto);
            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }
            return Ok(new { token });
        }
    }
}
