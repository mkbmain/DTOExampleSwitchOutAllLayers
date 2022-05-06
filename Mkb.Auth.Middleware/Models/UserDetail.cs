using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Mkb.Auth.Middleware.Models
{
    public class UserDetail
    {
        public UserDetail()
        {
        }

        public UserDetail(UserDetail userDetail)
        {
            this.UserId = userDetail.UserId;
            this.Roles = userDetail.Roles;
            this.Email = userDetail.Email;
        }

        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public Claim[] Claims => new[] { new Claim("Roles", string.Join(",", Roles)) }
            .Union(new[] { new Claim("Email", Email), new Claim("UserId", UserId.ToString("N")) })
            .ToArray();
    }
}