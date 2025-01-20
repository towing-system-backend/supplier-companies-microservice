using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyRif : IValueObject<SupplierCompanyRif>
    {
        private readonly string _value;

        public SupplierCompanyRif(string value)
        {
            if (!RifRegex.IsRif(value))
            {
                throw new InvalidSupplierCompanyRifException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(SupplierCompanyRif other) => _value == other._value;
    }
}
