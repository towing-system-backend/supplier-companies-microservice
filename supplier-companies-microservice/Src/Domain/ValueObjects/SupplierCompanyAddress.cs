using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyAddress : IValueObject<SupplierCompanyAddress>
    {
        private readonly string _state;
        private readonly string _city;
        private readonly string _street;

        public SupplierCompanyAddress(string state, string city, string street)
        {
            if (state.Length < 3 || state.Length > 20 ||
                city.Length < 3 || city.Length > 20 ||
                street.Length < 3 || street.Length > 20)
            {
                throw new InvalidSupplierCompanyAddressException();
            }

            _state = state;
            _city = city;
            _street = street;
        }

        public string GetState() => _state;
        public string GetCity() => _city;
        public string GetStreet() => _street;
        public bool Equals(SupplierCompanyAddress other) => _state == other._state && _city == other._city && _street == other._street;
    }
}
