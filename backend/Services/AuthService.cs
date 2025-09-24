using backend.Config;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Helpers; // This is now used for hashing/verification

namespace backend.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        // NOTE: Injecting the configuration for JWT secret
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. LOGIN: Finds user and verifies password using the PasswordHelper.
        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);

            // Check if user exists AND if the password is valid
            if (user == null || !PasswordHelper.Verify(password, user.PasswordHash))
            {
                return null; // Authentication failed
            }

            // Update last login date (Good practice)
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        // 2. VENDOR SUBMISSION: The vendor submits their full details via the verification link.
        public async Task<bool> SubmitVendorDetailsAsync(Guid token, string firstName, string lastName, string password)
        {
            // The vendor's User record is not created yet; we only look up the Vendor record
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VerificationToken == token);

            if (vendor == null || vendor.TokenExpiry < DateTime.UtcNow)
            {
                return false; // Invalid or expired token
            }

            // --- CRUCIAL LOGIC: CREATE THE USER ACCOUNT AND LINK IT ---

            // 1. Create the secure User record
            var hashedPassword = PasswordHelper.Hash(password); // Use the team's custom hasher
            var vendorRole = await _context.Roles.FirstAsync(r => r.Name == "Vendor");

            var vendorUser = new User
            {
                Email = vendor.ContactEmail, // Use the email stored on the vendor record
                PasswordHash = hashedPassword,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow,
                PublicId = Guid.NewGuid(),
                Roles = new List<Role> { vendorRole }
            };
            _context.Users.Add(vendorUser);
            await _context.SaveChangesAsync();

            // 2. Link the newly created User to the Vendor record
            vendor.UserId = vendorUser.Id;
            vendor.Status = "Active"; // Verification link submission should set status to Active
            vendor.VerificationToken = null; // Invalidate token
            vendor.TokenExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }

        // 3. (NEW) ADMIN & LEADER CREATION: Reusable method for creating any user role
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

        // 4. JWT TOKEN GENERATION: Remains mostly the same, but uses IConfiguration
        private string GenerateJwtToken(User user)
        {
            // Use IConfiguration to securely read environment variables
            var secretKey = _configuration["JWT_SECRET"];
            var issuer = _configuration["JWT_ISSUER"];
            var audience = _configuration["JWT_AUDIENCE"];

            // Ensure values are not null before using them (safety check)
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