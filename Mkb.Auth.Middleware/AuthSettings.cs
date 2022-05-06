namespace Mkb.Auth.Middleware
{
    public class AuthSettings
    {
        public int PasswordMinimumLength { get; set; }
        public bool PasswordMustBeMoreThanOneUniqueCharacterType { get; set; }
        public bool PasswordDuplicationForCharacterCheck { get; set; }
        public bool PasswordMustContainUpperAndLowerCaseLetter { get; set; }
    }
}