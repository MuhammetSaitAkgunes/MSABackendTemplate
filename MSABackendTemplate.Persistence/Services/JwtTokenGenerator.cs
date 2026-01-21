using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MSABackendTemplate.Persistence.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(ApplicationUser user, IList<string>? roles = null)
        {
            // appsettings.json'dan JWT ayarlarını okuyoruz
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var durationInMinutes = int.Parse(jwtSettings["DurationInMinutes"] ?? "60");

            // Security Key oluşturuyoruz (Symmetric key kullanıyoruz)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims Builder - Fluent API ile if statement'larını elimine ediyoruz
            var claims = new ClaimBuilder()
                .AddMandatory(ClaimTypes.NameIdentifier, user.Id)
                .AddMandatory(ClaimTypes.Email, user.Email!)
                .AddMandatory(ClaimTypes.Name, user.UserName!)
                .AddUniqueId(JwtRegisteredClaimNames.Jti)
                .AddOptional(ClaimTypes.GivenName, user.FirstName)
                .AddOptional(ClaimTypes.Surname, user.LastName)
                .AddRange(ClaimTypes.Role, roles)
                .Build();

            // Token oluşturma
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationInMinutes), // Token geçerlilik süresi
                signingCredentials: credentials
            );

            // Token'ı string olarak döndürüyoruz
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
