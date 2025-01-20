using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyType : IValueObject<PolicyType>
    {
        private readonly string _value;

        public PolicyType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidPolicyTypeException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(PolicyType other) => _value == other._value;
    }
}
