using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mkb.Auth.Contracts;
using Mkb.Auth.Contracts.DataStore;
using Mkb.Auth.Contracts.Dtos;
using Mkb.Auth.Contracts.StaticHelpers;
using Mkb.Auth.Middleware;
using Mkb.Auth.Middleware.Models;
using Mkb.Auth.Middleware.StaticHelpers;
using Mkb.Auth.Services.Models;
using Mkb.Auth.Services.Models.RequestsAndResponses.User;
using Mkb.Auth.Services.Models.UserModels;
using Mkb.Auth.Services.StaticHelpers;

namespace Mkb.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly AuthSettings _authSettings;
        private readonly TokenSettings _tokenSettings;
        private readonly IAuthDataStoreContract _authDataStore;

        public UserService(IOptions<AuthSettings> passwordSettings, IOptions<TokenSettings> tokenSettings,
            IAuthDataStoreContract dataStoreContract)
        {
            _authDataStore = dataStoreContract;
            _authSettings = passwordSettings.Value;
            _tokenSettings = tokenSettings.Value;
        }


        /// <inheritdoc />
        public async Task<CreateUserResponse> CreateUser(CreateUserRequest complex)
        {
            try
            {
                if (complex.Password == null || !complex.Password.ValidateForPassword(_authSettings))
                {
                    return new CreateUserResponse
                        {ResponseType = ResponseType.BadRequest, Message = "Password To Simple"};
                }

                if (complex.Email == null || !complex.Email.IsValidEmail())
                {
                    return new CreateUserResponse {ResponseType = ResponseType.BadRequest, Message = "Email Invalid"};
                }

                var existingUser = await GetUserDetailsByEmail(complex.Email);

                if (existingUser != null)
                {
                    return new CreateUserResponse
                        {ResponseType = ResponseType.BadRequest, Message = "Email Already In Use"};
                }

                var salt = PasswordHasher.GetSalt();
                var user = new UserDto
                {
                    UserId = Guid.NewGuid(),
                    FirstName = complex.FirstName ?? "",
                    LastName = complex.LastName ?? "",
                    Email = complex.Email.ToLower(),
                    Salt = salt,
                    Password = PasswordHasher.GenHash(Encoding.ASCII.GetBytes(salt), complex.Password)
                };
                await _authDataStore.Add(user, complex.Role);
                return new CreateUserResponse {ResponseType = ResponseType.Success, Message = "", UserId = user.UserId};
            }
            catch (Exception e)
            {
                return new CreateUserResponse {ResponseType = ResponseType.Error, Message = "Error has occured"};
            }
        }

        /// <inheritdoc />
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return new AuthenticateResponse(null, null)
                    {ResponseType = ResponseType.BadRequest, Message = "Email or password is missing"};
            }

            var user = await GetUserDetailsByEmail(model.Email);

            // if we have no user or password missmatch exit
            if (user == null ||
                user.Password != PasswordHasher.GenHash(Encoding.ASCII.GetBytes(user.Salt), model.Password))
            {
                return new AuthenticateResponse(null, null)
                    {ResponseType = ResponseType.BadRequest, Message = "Email or password is incorrect"};
            }

            // authentication successful so generate jwt token
            var token = TokenGen.GenerateJwtToken(user, _tokenSettings.Secret);

            return new AuthenticateResponse(user, token);
        }

        private async Task<ComplexUserDetails> GetUserDetailsByEmail(string email)
        {
            var (userDto, roles) = await _authDataStore.GetUserAndRoleByEmail(email);
            if (userDto == null)
            {
                return null;
            }

            return new ComplexUserDetails
            {
                UserId = userDto.UserId,
                Email = userDto.Email,
                Roles = roles?.Select(t => t.ToString()).ToList() ?? new List<string>(),
                Password = userDto.Password,
                Salt = userDto.Salt
            };
        }
    }
}