using Mkb.Auth.Contracts;

namespace Mkb.Auth.Services.Models.RequestsAndResponses.User
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; } = Roles.Customer;
    }
}