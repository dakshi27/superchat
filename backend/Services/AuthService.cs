using backend.Config;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Helpers;
using Microsoft.Extensions.Configuration;

namespace backend.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- CORE AUTHENTICATION AND LOGIN ---

        // Helper method to retrieve user and roles for JWT generation
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // 1. LOGIN: Finds user, verifies password, and generates JWT. (REQUIRED)
        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);

            // Check if user exists AND if the password is valid
            if (user == null || !PasswordHelper.Verify(password, user.PasswordHash))
            {
                return null; // Authentication failed
            }

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        // 2. TOKEN VERIFICATION: ONLY verifies the token's validity against the Vendor record. (REQUIRED)
        public async Task<Vendor?> VerifyVendorTokenAsync(Guid token)
        {
            // Finds a vendor record where the token is present AND the expiry date is in the future.
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v =>
                    v.VerificationToken == token &&
                    v.TokenExpiry > DateTime.UtcNow);

            return vendor;
        }

        public async Task<User?> CreateUserWithRoleAsync(string email, string password, string firstName, string lastName, string roleName)
        {
            // 1. Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return null; // User already exists
            }

            // 2. Get Role and Hash Password
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null) throw new Exception($"Role '{roleName}' not found.");

            var hashedPassword = PasswordHelper.Hash(password);

            // 3. Create User
            var newUser = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow,
                PublicId = Guid.NewGuid(),
                Roles = new List<Role> { role }
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }


        // --- HELPER METHODS FOR SECURITY/TOKEN GENERATION ---

        // Helper method to generate the JWT token
        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JWT_SECRET"];
            var issuer = _configuration["JWT_ISSUER"];
            var audience = _configuration["JWT_AUDIENCE"];

            if (string.IsNullOrEmpty(secretKey)) throw new Exception("JWT_SECRET is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}