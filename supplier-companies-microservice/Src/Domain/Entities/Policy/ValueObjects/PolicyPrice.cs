using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyPrice : IValueObject<PolicyPrice>
    {
        private readonly decimal _value;

        public PolicyPrice(decimal value)
        {
            if (value < 0)
            {
                throw new InvalidPolicyCoverageAmountException();
            }

            _value = value;
        }

        public decimal GetValue() => _value;
        public bool Equals(PolicyPrice other) => _value == other._value;
    }
}