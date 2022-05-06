using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Mkb.Auth.Middleware.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenSettings _tokenSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<TokenSettings> appSettings)
        {
            _next = next;
            _tokenSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers.TryGetValue("Authorization", out var token);

            if (!string.IsNullOrWhiteSpace(token)) await AttachUserToContext(context, token);

            await _next(context);
        }

        private Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                const string bearer = "bearer";
                token = token.ToLower().StartsWith(bearer) ? token[bearer.Length..].Trim() : token;
                if (context.Items.ContainsKey("Token")) context.Items.Remove("Token");

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;

                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    context.Response.Cookies.Delete("Auth");
                    throw new UnauthorizedAccessException();
                }

                var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, "Role", "Auth", "RoleType");
                context.User.AddIdentity(claimsIdentity);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}