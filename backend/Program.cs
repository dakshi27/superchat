/*using DotNetEnv;
using backend.Config;
using backend.Services;
using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// --- CORS policy name ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var emailConfig = new EmailConfig
{
    SendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
};
// singleton service so it can be injected into other classes.
builder.Services.AddSingleton(emailConfig);


// --- Add Custom Services ---
builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<LeadershipService>();
//builder.Services.AddScoped<AdminService>();
//builder.Services.AddScoped<EmailService>();
//builder.Services.AddScoped<VendorService>();

// Configuring the AWS S3
//var awsCredentials = new BasicAWSCredentials(
//    Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
//    Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
//);
//var awsConfig = new AmazonS3Config
//{
//    RegionEndpoint = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION"))
//};
//builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsCredentials, awsConfig));

// --- Add and Configure JWT Authentication ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // Allow requests from the default Angular development server origin.
                          // For production, you would replace this with your actual frontend domain.
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add a general description for the Swagger page
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My Vendor API", Version = "v1" });

    // 1. Define the security scheme (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // 2. Add a global security requirement to use the Bearer scheme
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // In production, use the custom error handler.
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Needed to map controller routes

app.Run();*/

using DotNetEnv;
using backend.Config;
using backend.Services;
using backend.Models; // Added this using statement
using backend.Helpers; // Added this using statement
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// --- CORS policy name ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var emailConfig = new EmailConfig
{
    SendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
};
// singleton service so it can be injected into other classes.
builder.Services.AddSingleton(emailConfig);


// --- Add Custom Services ---
builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<LeadershipService>();
//builder.Services.AddScoped<AdminService>();
//builder.Services.AddScoped<EmailService>();
//builder.Services.AddScoped<VendorService>();


// --- Add and Configure JWT Authentication ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                     policy =>
                     {
                         // Allow requests from the default Angular development server origin.
                         // For production, you would replace this with your actual frontend domain.
                         policy.WithOrigins("http://localhost:4200")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                     });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add a general description for the Swagger page
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My Vendor API", Version = "v1" });

    // 1. Define the security scheme (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // 2. Add a global security requirement to use the Bearer scheme
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // In production, use the custom error handler.
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Needed to map controller routes

// --- CRITICAL: APPLICATION INITIALIZATION FOR SEEDING ---
// THIS BLOCK WAS MISSING FROM YOUR CODE AND IS REQUIRED TO SEED THE DATABASE
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Ensure the database exists and all migrations are applied
        context.Database.Migrate();

        // Custom Seeding Logic to ensure the Admin user exists with the correct hash
        if (!context.Users.Any(u => u.Email == "admin@example.com"))
        {
            // Get the admin role. Assumes it exists from a previous migration or seed.
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");

            // --- IMPORTANT: Hash the password using the same method your app uses ---
            string passwordToHash = "hlo123";
            string hashedPassword = PasswordHelper.Hash(passwordToHash);

            var adminUser = new User
            {
                Email = "admin@example.com",
                PasswordHash = hashedPassword,
                FirstName = "Root",
                LastName = "Admin",
                CreatedAt = DateTime.UtcNow,
                PublicId = Guid.NewGuid(),
                Roles = new List<Role> { adminRole }
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
            Console.WriteLine("!!! Initial Admin User Seeded Successfully !!!");
        }
        else
        {
            Console.WriteLine("--- Admin user already exists. Skipping seeding. ---");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] An error occurred while seeding the database: {ex.Message}");
    }
}
// --- END APPLICATION INITIALIZATION ---

app.Run();