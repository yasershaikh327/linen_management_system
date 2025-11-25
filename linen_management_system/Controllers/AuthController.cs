using linen_management_system.DTOModels;
using linen_management_system.Interface;
using LinenManagement.Services;
using LinenManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinenManagement.Controllers
{
    
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(new { success = true, data = response });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }
        }

        [HttpPost("refresh")]

        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest(new { success = false, message = "Refresh token is required" });

            try
            {
                var response = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(new { success = true, data = response });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            await _authService.LogoutAsync(email);
            return Ok(new { success = true, message = "Logged out" });
        }
    }
}