using System;
using Microsoft.AspNetCore.Mvc;
using Mkb.Auth.Middleware.Middleware;

namespace AuthApiExample.Controllers
{
    public class BaseController : Controller
    {
        public Guid GetUserId()
        {
            return Guid.Parse(ExtractClaimFromUser.GetRole(HttpContext,"UserId"));
        }

        public string GetUserEmail()
        {
            return ExtractClaimFromUser.GetRole(HttpContext,"Email");
        }


    }
}