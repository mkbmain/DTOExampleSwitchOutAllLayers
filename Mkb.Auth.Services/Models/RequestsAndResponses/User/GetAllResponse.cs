using Mkb.Auth.Contracts;
using Mkb.Auth.Middleware.Models;

namespace Mkb.Auth.Services.Models.RequestsAndResponses.User
{
    public class GetAllResponse : BaseResponse
    {
        public int Total { get; set; }
        public UserDetail[] PeoplePaged { get; set; }
    }
}