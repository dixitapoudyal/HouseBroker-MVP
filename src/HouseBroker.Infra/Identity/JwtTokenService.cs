using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Infra.Identity
{
    public class JwtTokenService
    {
        public readonly JWTConfiguration _jwtConfig;
        public JwtTokenService(IOptions<JWTConfiguration> jwtconfig)
        {
                _jwtConfig = jwtconfig.Value;
        }
        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("Name", user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // role claims so [Authorize(Roles = "Broker")] works downstream
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
