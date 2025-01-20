using Application.Core;

namespace SupplierCompany.Domain
{
    public class DepartmentName : IValueObject<DepartmentName>
    {
        private readonly string _value;

        public DepartmentName(string value)
        {
            if (value.Length < 5 || value.Length > 50)
            {
                throw new InvalidDepartmentNameException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(DepartmentName other) => _value == other._value;
    }
}
