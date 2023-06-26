using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConfigMan.Data;
using Microsoft.IdentityModel.Tokens;

namespace ConfigMan.Service
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationService _applicationService;

        public AuthService(IConfiguration configuration, IApplicationService applicationService)
        {
            _configuration = configuration;
            _applicationService = applicationService;
        }

        public string GenerateJwtToken(string userId, string role)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }

        public async Task<string> GenerateApplicationTokenAsync(string applicationName)
        {
            // Retrieve the token associated with the application
            var application = await _applicationService.GetApplicationByIdAsync(applicationName);
            if (application == null)
            {
                throw new ArgumentException("Invalid application ID");
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(application.Token));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, applicationName),
                new Claim("ApplicationName", applicationName) 
            };

            var tokenOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
    }
}

