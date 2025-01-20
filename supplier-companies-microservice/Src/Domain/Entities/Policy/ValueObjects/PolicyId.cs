using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyId : IValueObject<PolicyId>
    {
        private readonly string _value;

        public PolicyId(string value)
        {
            if (!GuidEx.IsGuid(value))
            {
                throw new InvalidPolicyIdException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(PolicyId other) => _value == other._value;
    }
}
