using backend.Models;
using backend.DTOs;
using System.Threading.Tasks;

namespace backend.Services
{
    public class AdminService
    {
        private readonly AuthService _authService;

        public AdminService(AuthService authService)
        {
            _authService = authService;
        }

        // REQUIRED: Admins add Leaders.
        public async Task<User?> AddLeaderAsync(CreateUserRequest request)
        {
            // The AdminService calls AuthService to handle the secure creation.
            // The role is explicitly set to "Leadership" here.
            var newLeader = await _authService.CreateUserWithRoleAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                "Leadership"
            );

            return newLeader;
        }
    }
}