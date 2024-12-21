using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyExpirationDate : IValueObject<PolicyExpirationDate>
    {
        private readonly DateTime _value;

        public PolicyExpirationDate(DateTime value)
        {
            if (value < DateTime.Now)
            {
                throw new InvalidPolicyExpirationDateException();
            }

            _value = value;
        }

        public DateTime GetValue() => _value;
        public bool Equals(PolicyExpirationDate other) => _value == other._value;
    }
}
