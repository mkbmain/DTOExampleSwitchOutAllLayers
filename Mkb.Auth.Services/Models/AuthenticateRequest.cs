namespace Mkb.Auth.Services.Models
{
    public class AuthenticateRequest
    {
        public string Message { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
    }
}