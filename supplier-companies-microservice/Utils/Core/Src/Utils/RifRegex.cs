using System.Text.RegularExpressions;

namespace Application.Core
{
    public static class RifRegex
    {
        public static bool IsRif(string rif)
        {
            return Regex.IsMatch(rif, @"^J-\d{8}-\d$");
        }
    }
}
