using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyCoverageDistance : IValueObject<PolicyCoverageDistance>
    {
        private readonly int _value;

        public PolicyCoverageDistance(int value)
        {
            if (value < 0)
            {
                throw new InvalidPolicyCoverageAmountException();
            }

            _value = value;
        }

        public int GetValue() => _value;
        public bool Equals(PolicyCoverageDistance other) => _value == other._value;
    }
}
