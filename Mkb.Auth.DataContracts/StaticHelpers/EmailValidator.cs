using System;
using System.Net.Mail;

namespace Mkb.Auth.Contracts.StaticHelpers
{
    public static class EmailValidator
    {
        public static bool IsValidEmail(this  string emailaddress)
        {
            try
            {
                var m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}