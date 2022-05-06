using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mkb.Auth.Contracts;
using Mkb.Auth.Contracts.DataStore;
using Mkb.Auth.Contracts.Dtos;
using Mkb.Auth.DataMongo.Repo;

namespace Mkb.Auth.DataMongo
{
    public class AuthDataStoreMongo : IAuthDataStoreContract
    {
        private readonly IMongoRepository _repository;

        public AuthDataStoreMongo(IMongoRepository repository)
        {
            _repository = repository;
        }

        public async Task<(UserDto, IEnumerable<Roles>)> GetUserAndRoleByEmail(string email)
        {
            var item = await _repository.GetAllAsync<AuthUserMongo>(t => t.Email.ToLower() == email.ToLower());

            var userDto = item.Select(t => new
                {
                    roles = t.RoleIds.Cast<Roles>(),
                    dto = new UserDto
                    {
                        UserId = t.UserId,
                        Email = t.Email,
                        Password = t.Password,
                        Salt = t.Salt,
                        FirstName = t.FirstName,
                        LastName = t.LastName
                    }
                }
            ).FirstOrDefault();

            return (userDto?.dto, userDto?.roles);
        }

        public async Task<string> Add(UserDto userDto, Roles withrole)
        {
            var user = new AuthUserMongo()
            {
                Email = userDto.Email,
                UserId = userDto.UserId,
                Password = userDto.Password,
                Salt = userDto.Salt,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                RoleIds = new[] {(int) withrole}
            };

            await _repository.Add(user);
            return user.Id.ToString();
        }
    }
}