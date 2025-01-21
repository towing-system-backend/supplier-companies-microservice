using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyIssuanceDate : IValueObject<PolicyIssuanceDate>
    {
        private readonly DateOnly _value;

        public PolicyIssuanceDate(DateOnly value)
        {
            if (value > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new InvalidPolicyIssuanceDateException();
            }

            _value = value;
        }

        public DateOnly GetValue() => _value;
        public bool Equals(PolicyIssuanceDate other) => _value == other._value;
    }
}