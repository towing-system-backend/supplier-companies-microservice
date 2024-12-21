using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyType : IValueObject<SupplierCompanyType>
    {
        private readonly string _value;
        private static readonly string[] ValidTypes = { "Internal", "External" };

        public SupplierCompanyType(string value)
        {
            if (!IsValidType(value))
            {
               throw new InvalidSupplierCompanyTypeException();
            }

            _value = value;
        }

        private static bool IsValidType(string value)
        {
            return Array.Exists(ValidTypes, status => status.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public string GetValue() => _value;
        public bool Equals(SupplierCompanyType other) => _value == other._value;
    }
}