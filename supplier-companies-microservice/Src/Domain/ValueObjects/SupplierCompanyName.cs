using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyName : IValueObject<SupplierCompanyName>
    {
        private readonly string _value;

        public SupplierCompanyName(string value)
        {
            if (value.Length < 5 || value.Length > 20)
            {
                throw new InvalidSupplierCompanyNameException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(SupplierCompanyName other) => _value == other._value;
    }
}
