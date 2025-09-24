using backend.Services;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Authenticate the user and get the JWT token
            var token = await _authService.LoginAsync(request.Email, request.Password);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 2. Get user details for the response payload
            var user = await _authService.GetUserByEmailAsync(request.Email);

            // Safety check: should not be null if LoginAsync succeeded
            if (user == null || !user.Roles.Any())
            {
                return Unauthorized(new { message = "User found but role not assigned." });
            }

            // 3. Return the token and user details
            var response = new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Role = user.Roles.First().Name // Assuming a user has only one primary role for simplicity
            };

            return Ok(response);
        }

        // POST: api/Auth/submit-vendor-details
        // This is the endpoint the vendor hits after clicking the verification link.
        [HttpPost("submit-vendor-details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitVendorDetails([FromQuery] Guid token, [FromBody] RegisterLeaderRequest request)
        {
            try
            {
                var success = await _authService.SubmitVendorDetailsAsync(
                    token,
                    request.FirstName,
                    request.LastName,
                    request.Password
                );

                if (success)
                {
                    return Ok(new { message = "Vendor details submitted and account activated successfully. You can now log in." });
                }

                return BadRequest(new { message = "Invalid or expired verification link." });
            }
            catch (Exception ex)
            {
                // In a real app, you would log the exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}