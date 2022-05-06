using System;
using Mkb.Auth.Contracts;
using Mkb.Auth.Middleware.Models;

namespace Mkb.Auth.Services.Models
{
    public class AuthenticateResponse : BaseResponse
    {
        public Guid UserGuid { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(UserDetail user, string token)
        {
            UserGuid = user?.UserId ?? Guid.Empty;
            Email = user?.Email;
            Token = token;
        }
    }
}