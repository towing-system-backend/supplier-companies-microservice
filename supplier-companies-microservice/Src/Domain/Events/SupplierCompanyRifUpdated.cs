using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyRifUpdatedEvent(string publisherId, string type, SupplierCompanyRifUpdated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyRifUpdated(string rif)
    {
        public readonly string Rif = rif;

        public static SupplierCompanyRifUpdatedEvent CreateEvent(SupplierCompanyId publisherId, SupplierCompanyRif rif)
        {
            return new SupplierCompanyRifUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyRifUpdated).Name,
                new SupplierCompanyRifUpdated(
                    rif.GetValue()
                )
            );
        }
    }
}
