using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyPhoneNumber : IValueObject<SupplierCompanyPhoneNumber>
    {
        private readonly string _value;

        public SupplierCompanyPhoneNumber(string value)
        {
            if (!PhoneNumberRegex.IsPhoneNumber(value))
            {
                throw new InvalidSupplierCompanyPhoneNumberException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(SupplierCompanyPhoneNumber other) => _value == other._value;
    }
}
