using System.Text.RegularExpressions;

namespace DeltaWebApi.Helpers
{
    public class PasswordPolicy
    {
        private static int Minimum_Length = 8;
        private static int Upper_Case_length = 1;
        private static int Lower_Case_length = 1;
        private static int NonAlpha_length = 0;
        private static int Numeric_length = 1;

        public static bool IsValid(string Password, out string messages)
        {
            messages = "";
            if (Password.Length < Minimum_Length)
            {
                messages = $"Lütfen şifrenin en az {Minimum_Length} karakter olduğundan emin olunuz." + System.Environment.NewLine;
                return false;
            }
            if (UpperCaseCount(Password) < Upper_Case_length)
            {
                messages += $"Lütfen şifrenin en az {Upper_Case_length} adet büyük harf karakter içerdiğinden emin olunuz." + System.Environment.NewLine;
                return false;
            }
            if (LowerCaseCount(Password) < Lower_Case_length)
            {
                messages += $"Lütfen şifrenin en az {Lower_Case_length} adet küçük harf karakter içerdiğinden emin olunuz." + System.Environment.NewLine;
                return false;
            }
            if (NumericCount(Password) < Numeric_length)
            {
                messages += $"Lütfen şifrenin en az {Numeric_length} adet sayısal karakter içerdiğinden emin olunuz." + System.Environment.NewLine;
                return false;
            }
            if (NonAlphaCount(Password) < NonAlpha_length)
            {
                messages += $"Lütfen şifrenin en az {Numeric_length} adet özel karakter içerdiğinden emin olunuz." + System.Environment.NewLine;
                return false;
            }

            return true;
        }

        public static string PolicyMessages(string locale)
        {
            string str = "";
            if (locale.Equals("tr"))
            {
                str = @"Lütfen şifreniz en az {Upper_Case_length} karakter büyük harf , " +
                    "en az {Lower_Case_length} karakter küçük harf ,en az {Numeric_length} karekter sayı, en az 1 karakter özel karakter içerdiğinden ve en az 1 karakter uzunlukta olduğundan emin olunuz.";
            }
            else
            {
                str = @"Please check your password includes minimum {Upper_Case_length} Upper case char , minimum {} Lover case char , minimum {Numeric_length} numeric char en " +
                       " ";

            }

            return str;
        }

        private static int UpperCaseCount(string Password)
        {
            return Regex.Matches(Password, "[A-Z]").Count;
        }

        private static int LowerCaseCount(string Password)
        {
            return Regex.Matches(Password, "[a-z]").Count;
        }

        private static int NumericCount(string Password)
        {
            return Regex.Matches(Password, "[0-9]").Count;
        }

        private static int NonAlphaCount(string Password)
        {
            return Regex.Matches(Password, @"[^0-9a-zA-Z\._]").Count;
        }

    }
}