using Application.Core;

namespace SupplierCompany.Domain
{
    public class DepartmentId : IValueObject<DepartmentId>
    {
        private readonly string _value;

        public DepartmentId(string value)
        {
            if (!GuidEx.IsGuid(value))
            {
                throw new InvalidDepartmentIdException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(DepartmentId other) => _value == other._value;
    }
}