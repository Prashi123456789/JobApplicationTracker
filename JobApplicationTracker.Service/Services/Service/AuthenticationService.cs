using JobApplicationTracker.Service.Configuration;
using JobApplicationTracker.Service.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobApplicationTracker.Data.DataModels;

namespace JobApplicationTracker.Service.Services.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(IOptions<JwtSettings> settings)
        {
            _jwtSettings = settings.Value;
        }

        public string GenerateJwtToken(UsersDataModel user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email),  // Use ClaimTypes for common claims
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Use ClaimTypes for consistency
            };

            // The custom key that we have set in the appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            // Create signingCredentials and hash it with an algorithm
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create an instance of JwtSecurityToken for creating a token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes), // Use DateTime.UtcNow for consistency
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}