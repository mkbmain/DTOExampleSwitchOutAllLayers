using System;
using Mkb.Auth.DataMongo.Repo;

namespace Mkb.Auth.DataMongo
{
    internal class AuthUserMongo : MongoEntity
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int[] RoleIds { get; set; }
    }
}