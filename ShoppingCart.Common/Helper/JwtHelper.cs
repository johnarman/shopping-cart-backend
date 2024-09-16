using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Helper;
using ShoppingCart.Entity.Model;
using Microsoft.IdentityModel.Tokens;

namespace Common.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private readonly string _key;
        private readonly string _issuer;

        public JwtHelper(string key, string issuer)
        {
            _key = key;
            _issuer = issuer;
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                _issuer,
                _issuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static int GetUserIdFromToken(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User is not authorized.");
            }

            return int.Parse(userIdClaim);
        }

    }
}
