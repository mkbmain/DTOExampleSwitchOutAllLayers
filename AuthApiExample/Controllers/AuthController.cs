using Microsoft.AspNetCore.Mvc;
using Mkb.Auth.Middleware.Middleware;

namespace AuthApiExample.Controllers
{
    [ApiController]
    [Route("{Controller}")]
    public class AuthController : BaseController
    {
        [HttpGet("Test")]
        [Authorize("*")] // << role required can be split with , i.e admin,manager,god 
        // or * wild card for any one that has a valid token regardless of role
        public IActionResult Get()
        {
            return Ok(new {Status = "You Are Authed"});
        }
    }
}