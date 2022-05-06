using System.Text.Json.Serialization;
using Mkb.Auth.Middleware.Models;

namespace Mkb.Auth.Services.Models.UserModels
{
    public class ComplexUserDetails : UserDetail
    {
        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public string Salt { get; set; }
    }
}