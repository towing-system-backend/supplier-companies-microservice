using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyAddressUpdatedEvent(string publisherId, string type, SupplierCompanyAddressUpdated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyAddressUpdated(string state, string city, string street)
    {
        public readonly string State = state;
        public readonly string City = city;
        public readonly string Street = street;
        public static SupplierCompanyAddressUpdatedEvent CreateEvent(SupplierCompanyId publisherId, SupplierCompanyAddress address)
        {
            return new SupplierCompanyAddressUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyAddressUpdated).Name,
                new SupplierCompanyAddressUpdated(
                    address.GetState(),
                    address.GetCity(),
                    address.GetStreet()
                )
            );
        }
    }
}
