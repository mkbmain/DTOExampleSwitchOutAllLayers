using System.Linq;

namespace Mkb.Auth.Middleware.Middleware
{
    public static class ExtractClaimFromUser
    {
        public static string GetRole(Microsoft.AspNetCore.Http.HttpContext context,string roleName)
        {
            return context.User.Claims.FirstOrDefault(f => f.Type == roleName)?.Value;
        }
    }
}