using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyNameUpdatedEvent(string publisherId, string type, SupplierCompanyNameUpdated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyNameUpdated(string name)
    {
        public readonly string Name = name;
        public static SupplierCompanyNameUpdatedEvent CreateEvent(SupplierCompanyId publisherId, SupplierCompanyName name)
        {
            return new SupplierCompanyNameUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyNameUpdated).Name,
                new SupplierCompanyNameUpdated(
                    name.GetValue()
                )
            );
        }
    }
}
