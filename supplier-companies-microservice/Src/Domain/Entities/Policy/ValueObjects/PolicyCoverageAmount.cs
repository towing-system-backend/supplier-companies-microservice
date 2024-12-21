using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyCoverageAmount : IValueObject<PolicyCoverageAmount>
    {
        private readonly int _value;

        public PolicyCoverageAmount(int value)
        {
            if (value < 0)
            {
                throw new InvalidPolicyCoverageAmountException();
            }

            _value = value;
        }

        public int GetValue() => _value;
        public bool Equals(PolicyCoverageAmount other) => _value == other._value;
    }
}
