using backend.Config;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Helpers;

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
            Console.WriteLine($"[DEBUG] User found: {user != null}");

            if (user == null)
            {
                return null; // Authentication failed (User not found)
            }

            // --- CRUCIAL DEBUGGING LINES ---
            // Debug line 2: Display the password hash retrieved from the database
            Console.WriteLine($"[DEBUG] DB Hash: '{user.PasswordHash}'");

            // Debug line 3: Attempt to hash the input password to see what it LOOKS like
            // NOTE: This is NOT the Verify step, just seeing the input data
            string inputHashFormat = PasswordHelper.Hash(password);
            Console.WriteLine($"[DEBUG] Input Password: '{password}'");
            // This line won't show the exact comparison hash, but confirms the input string.
            // -------------------------------

            // Check if user exists AND if the password is valid
            if (!PasswordHelper.Verify(password, user.PasswordHash))
            {
                // Login failed due to comparison mismatch
                Console.WriteLine("[DEBUG] VERIFY FAILED!");
                return null;
            }

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        // 2. TOKEN VERIFICATION: ONLY verifies the token's validity against the Vendor record. (REQUIRED)
        /*public async Task<Vendor?> VerifyVendorTokenAsync(Guid token)
        {
            // Finds a vendor record where the token is present AND the expiry date is in the future.
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v =>
                    v.VerificationToken == token &&
                    v.TokenExpiry > DateTime.UtcNow);
            // add the logic pleaseeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
            return vendor;
        }*/

        public async Task<Vendor?> VerifyVendorTokenAsync(Guid token)
        {
            // Finds a vendor record where the token is present AND the expiry date is in the future.
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v =>
                    v.VerificationToken == token &&
                    v.TokenExpiry > DateTime.UtcNow);

            // ?? LOGIC ADDED HERE: Update the status to show the link has been verified/used.
            if (vendor != null)
            {
                // We only update the status if it's currently 'Pending' or 'Initial'
                // This marks that the user has successfully clicked the link.
                if (vendor.Status == "Pending")
                {
                    vendor.Status = "Verified"; // Set a new status to track token usage
                    vendor.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            return vendor;
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