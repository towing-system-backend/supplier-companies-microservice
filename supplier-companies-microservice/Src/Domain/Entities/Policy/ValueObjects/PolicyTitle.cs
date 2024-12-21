using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyTitle : IValueObject<PolicyTitle>
    {
        private readonly string _value;

        public PolicyTitle(string value)
        {
            if (value.Length < 5 || value.Length > 20)
            {
                throw new InvalidSupplierCompanyNameException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(PolicyTitle other) => _value == other._value;
    }
}