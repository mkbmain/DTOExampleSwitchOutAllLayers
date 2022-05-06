using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mkb.Auth.Contracts;
using Mkb.Auth.Contracts.DataStore;
using Mkb.Auth.Contracts.Dtos;

namespace Mkb.Auth.DataSql
{

    public class AuthDataStoreSql : IAuthDataStoreContract
    {
        private readonly AuthDbContext _dbContext;

        public AuthDataStoreSql(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(UserDto, IEnumerable<Roles>)> GetUserAndRoleByEmail(string email)
        {
            var item = await _dbContext.Users.Where(t => t.Email.ToLower() == email.ToLower())
                .Select(t => new
                {
                    userDetails = t,
                    roles = t.UserRoles.Select(e => e.RoleId)
                }).FirstOrDefaultAsync();

            return (item?.userDetails, item?.roles?.Cast<Roles>().ToArray() ?? Array.Empty<Roles>());
        }

        public async Task<string> Add(UserDto userDto, Roles withrole)
        {
            var user = new User
            {
                UserRoles = new List<UserRoles>
                {
                    new UserRoles
                    {
                        RoleId = (int) withrole,
                    }
                },
                Password = userDto.Password,
                Email = userDto.Email,
                FirstName = userDto.Email,
                Salt = userDto.Salt,
                LastName = userDto.LastName,
                UserId = userDto.UserId,
            };
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user.Id.ToString();
        }
    }
}