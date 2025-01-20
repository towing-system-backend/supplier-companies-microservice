using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyId : IValueObject<SupplierCompanyId>
    {
        private readonly string _value;

        public SupplierCompanyId(string value)
        {
            if (!GuidEx.IsGuid(value))
            {
                throw new InvalidSupplierCompanyIdException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(SupplierCompanyId other) => _value == other._value;
    }
}
