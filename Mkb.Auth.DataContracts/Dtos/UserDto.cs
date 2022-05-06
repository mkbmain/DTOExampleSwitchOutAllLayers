using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mkb.Auth.Contracts.Dtos
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore] public string Salt { get; set; }
        public string Email { get; set; }
        [JsonIgnore] public string Password { get; set; }
    }
}