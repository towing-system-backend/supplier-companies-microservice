using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyExpirationDate : IValueObject<PolicyExpirationDate>
    {
        private readonly DateOnly _value;

        public PolicyExpirationDate(DateOnly value)
        {
            if (value < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidPolicyExpirationDateException();
            }

            _value = value;
        }

        public DateOnly GetValue() => _value;
        public bool Equals(PolicyExpirationDate other) => _value == other._value;
    }
}
