using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyIssuanceDate : IValueObject<PolicyIssuanceDate>
    {
        private readonly DateTime _value;

        public PolicyIssuanceDate(DateTime value)
        {
            if (value < DateTime.Now)
            {
                throw new InvalidPolicyIssuanceDateException();
            }

            _value = value;
        }

        public DateTime GetValue() => _value;
        public bool Equals(PolicyIssuanceDate other) => _value == other._value;
    }
}