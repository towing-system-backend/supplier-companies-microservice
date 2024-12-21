using System.Text.RegularExpressions;

namespace Application.Core
{
    public static class PhoneNumberRegex
    {
        public static bool IsPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^(0?4(14|24|16|26)\d{7})$");
        }
    }
}
