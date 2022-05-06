using System.Collections.Generic;
using System.Threading.Tasks;
using Mkb.Auth.Middleware.Models;
using Mkb.Auth.Services.Models;
using Mkb.Auth.Services.Models.RequestsAndResponses.User;

namespace Mkb.Auth.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Auths a user and gives a jwt token
        /// </summary>
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        

        /// <summary>
        /// Create User
        /// </summary>
        Task<CreateUserResponse> CreateUser(CreateUserRequest complex);
    }
}