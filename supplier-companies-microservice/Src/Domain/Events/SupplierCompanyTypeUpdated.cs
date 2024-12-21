using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyTypeUpdatedEvent(string publisherId, string type, SupplierCompanyTypeUpdated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyTypeUpdated(string type)
    {
        public readonly string Type = type;

        public static SupplierCompanyTypeUpdatedEvent CreateEvent(SupplierCompanyId publisherId, SupplierCompanyType type)
        {
            return new SupplierCompanyTypeUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyTypeUpdated).Name,
                new SupplierCompanyTypeUpdated(
                    type.GetValue()
                )
            );
        }
    }
}
