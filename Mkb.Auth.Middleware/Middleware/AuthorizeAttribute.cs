using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mkb.Auth.Middleware.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private string _arg;

        public AuthorizeAttribute(string arg)
        {
            _arg = arg;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var roles = ExtractClaimFromUser.GetRole(context.HttpContext, "Roles")?.Split(",");
            var args = _arg.Split(",");
            if (roles == null || !roles.Any() || _arg != null && !roles.Any(t => args.Any(a => a == t)) &&
                args.Any(t => t.Contains("*") == false))
                context.Result = new ForbidResult();
        }
    }
}