using JobApplicationTracke.Data.Database;
using JobApplicationTracker.Api.GlobalExceptionHandler;
using JobApplicationTracker.Data.Config;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Service;
using JobApplicationTracker.Service.Configuration;
using JobApplicationTracker.Service.Services.Interfaces;
using JobApplicationTracker.Service.Services.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with global exception handler
builder.Services.AddControllers(config =>
{
    config.Filters.AddService<GlobalExceptionHandler>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// Register GlobalExceptionHandler as a service
builder.Services.AddScoped<GlobalExceptionHandler>();

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configure Database settings
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("ConnectionStrings")); // Add this line
builder.Services.AddScoped<IDatabaseConnectionService, DatabaseConnectionService>(); // Add this line

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add security definition for JWT
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token."
    });

    // Add security requirement
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// CORS policy for all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials());
});

// Authentication service configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidAudience = jwtSettings?.Audience,
        ValidIssuer = jwtSettings?.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key ?? "")),
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
    };
})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.AccessDeniedPath = "/";
    options.LogoutPath = "/";
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add service layer dependencies
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

// Calling the extension method to register all services from Service and Data layers
builder.Services.AddServiceLayer(builder.Configuration);

// Build the application
var app = builder.Build();

// Enable CORS
app.UseCors("AllowAllOrigins");

// Configure Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "JobApplicationTracker API V1");
        options.RoutePrefix = string.Empty; // This makes Swagger UI the root
    });
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthentication(); // Ensure authentication middleware is enabled
app.UseAuthorization(); // Ensure to add this for authorization

app.MapControllers(); // Map controller routes

// Run the application
app.Run();