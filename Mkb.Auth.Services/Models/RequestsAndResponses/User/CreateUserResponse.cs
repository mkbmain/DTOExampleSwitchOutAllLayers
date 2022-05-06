using System;
using Mkb.Auth.Contracts;

namespace Mkb.Auth.Services.Models.RequestsAndResponses.User
{
    public class CreateUserResponse :BaseResponse
    {
        public Guid UserId { get; set; }
    }
}