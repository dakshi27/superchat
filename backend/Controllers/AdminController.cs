using backend.Services;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    // Restrict access to this entire controller to users with the 'Admin' role
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // POST: api/Admin/leader
        // Endpoint for Admin to create a new Leadership user account.
        [HttpPost("leader")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Handled by [Authorize]
        public async Task<IActionResult> AddLeader([FromBody] CreateUserRequest request)
        {
            try
            {
                var newLeader = await _adminService.AddLeaderAsync(request);

                if (newLeader == null)
                {
                    // User already exists
                    return BadRequest(new { message = "User with this email already exists." });
                }

                // Return 201 Created and the details of the new leader
                return CreatedAtAction(
                    nameof(AddLeader),
                    new { id = newLeader.Id },
                    new UserDto(newLeader.Id, newLeader.Email, newLeader.FirstName, newLeader.LastName)
                );
            }
            catch (Exception ex)
            {
                // Catch any exceptions (like role not found, etc.)
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}