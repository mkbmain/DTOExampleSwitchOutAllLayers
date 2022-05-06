using System.Collections.Generic;
using System.Linq;
using Mkb.Auth.Middleware;

namespace Mkb.Auth.Services.StaticHelpers
{
    public static class PasswordValidator
    {
        private static Dictionary<char, bool> _lowerCase =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().GroupBy(f => f).ToDictionary(x => x.Key, x => true);

        private static Dictionary<char, bool> _upperCase =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ".GroupBy(f => f).ToDictionary(x => x.Key, x => true);

        private static Dictionary<char, bool>[] _lookUpTyps =
        {
            _lowerCase,
            _upperCase,
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ".GroupBy(x => x).ToDictionary(x => x.Key, x => true),
            "1234567890".GroupBy(f => f).ToDictionary(x => x.Key, x => true)
        };

        public static bool ValidateForPassword(this string password, AuthSettings settings)
        {
            if (settings.PasswordMinimumLength > password.Length)
            {
                return false;
            }

            if (settings.PasswordDuplicationForCharacterCheck)
            {
                var minAmount = settings.PasswordMinimumLength * 0.7;
                if (password.GroupBy(x => x).Count() < minAmount)
                {
                    return false;
                }
            }

            if (settings.PasswordMustContainUpperAndLowerCaseLetter)
            {
                var containsLowercase = password.Select(f => _lowerCase.ContainsKey(f)).Any(f => f);
                var containsUppercase = password.Select(f => _upperCase.ContainsKey(f)).Any(f => f);
                if (!containsLowercase || !containsUppercase)
                {
                    return false;
                }
            }

            if (settings.PasswordMustBeMoreThanOneUniqueCharacterType)
            {
                var index = new List<int>();
                foreach (var letter in password)
                {
                    bool found = false;
                    for (int i = 0; i < _lookUpTyps.Length; i++)
                    {
                        if (_lookUpTyps[i].ContainsKey(letter))
                        {
                            found = true;
                            index.Add(i);
                            break;
                        }
                    }

                    if (found == false)
                    {
                        index.Add(_lookUpTyps.Length);
                    }
                }

                if (index.Distinct().Count() < 2)
                {
                    return false;
                }
            }

            return true;
        }
    }
}