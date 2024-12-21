using Application.Core;

namespace SupplierCompany.Domain
{
    public class TowDriverId : IValueObject<TowDriverId>
    {
        private readonly string _value;

        public TowDriverId(string value)
        {
            if (!GuidEx.IsGuid(value))
            {
                throw new InvalidTowDriverIdException();
            }

            _value = value;
        }

        public string GetValue() => _value;
        public bool Equals(TowDriverId other) => _value == other._value;
    }
}