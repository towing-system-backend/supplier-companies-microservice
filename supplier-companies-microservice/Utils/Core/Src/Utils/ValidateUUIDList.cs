using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Core
{
    public class ValidateEachGuidAttribute : ValidationAttribute
    {
        private static readonly Regex _guidRegex = new Regex(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", RegexOptions.Compiled);

        public override bool IsValid(object value)
        {
            if (value is null) return true;
            if (value is IEnumerable<string> guids)
            {
                foreach (var guid in guids)
                {
                    if (!_guidRegex.IsMatch(guid))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
