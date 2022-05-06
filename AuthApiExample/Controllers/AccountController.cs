using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mkb.Auth.Contracts;
using Mkb.Auth.Services;
using Mkb.Auth.Services.Models;
using Mkb.Auth.Services.Models.RequestsAndResponses.User;

namespace AuthApiExample.Controllers
{
    [ApiController]
    [Route("{Controller}")]
    public class AccountController : BaseController
    {
        private IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]AuthenticateRequest request)
        {
            var response = await _userService.Authenticate(request);
            if (response.ResponseType != ResponseType.Success)
            {
                return BadRequest(response); // for a test I am fine with this if this was real i would not return message back as might contain sensetive info
            }
            return Ok(response); // for a test I am fine with this if this was real i would not return message back as might contain sensetive info
        }
        
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var response = await _userService.CreateUser(request);

            if (response.ResponseType != ResponseType.Success)
            {
                return BadRequest(response);            // for a test I am fine with this if this was real i would not return message back as might contain sensetive info
            }
            return Ok(response); // for a test I am fine with this if this was real i would not return message back as might contain sensetive info
        }
    }
}