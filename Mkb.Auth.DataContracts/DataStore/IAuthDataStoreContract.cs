using System.Collections.Generic;
using System.Threading.Tasks;
using Mkb.Auth.Contracts.Dtos;

namespace Mkb.Auth.Contracts.DataStore
{
    public interface IAuthDataStoreContract
    {
         Task<string> Add(UserDto userDto, Roles withrole);

         Task<(UserDto, IEnumerable<Roles>)> GetUserAndRoleByEmail(string email);
    }
}