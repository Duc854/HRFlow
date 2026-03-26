using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Business.Abstractions;
using Data.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;

namespace Business.InternalServices
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly JwtSettings _jwtSettings;
        public JwtTokenProvider(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }
        public string GenerateAccessToken(User user)
        {
            var jwtKey = _jwtSettings.Key ?? throw new InvalidOperationException("JWT Key is not configured.");
            var key = Encoding.UTF8.GetBytes(jwtKey);
            var expireMinutes = int.Parse(_jwtSettings.ExpireMinutes ?? "60");

            // Khởi tạo danh sách Claims cơ bản
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("EmployeeId", user.EmployeeId?.ToString() ?? ""),
                    new Claim("DepartmentId", user.Employee?.DepartmentId.ToString() ?? ""),
                };

            // Lấy tất cả Roles của User và thêm vào Claims
            if (user.UserRoles != null)
            {
                var roles = user.UserRoles.Select(ur => ur.Role.Name);
                foreach (var roleName in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
