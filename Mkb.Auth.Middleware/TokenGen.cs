using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Mkb.Auth.Middleware.Models;

namespace Mkb.Auth.Middleware
{
    public static class TokenGen
    {
        public static string GenerateJwtToken(UserDetail user, string secret,DateTime? expiresAt = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = user.Claims.GroupBy(f=>f.Type).ToDictionary(f=>f.Key,f=> (object)f.First().Value),
                Subject = new ClaimsIdentity(user.Claims),
                Expires = expiresAt ?? DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}