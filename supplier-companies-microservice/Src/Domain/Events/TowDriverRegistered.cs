using Application.Core;

namespace SupplierCompany.Domain
{
    public class TowDriverRegisteredEvent(string publisherId, string type, TowDriverRegistered context) : DomainEvent(publisherId, type, context) { }

    public class TowDriverRegistered(string id)
    {
        public readonly string Id = id;

        public static TowDriverRegisteredEvent CreateEvent(SupplierCompanyId publisherId, UserId towDriverId)
        {
            return new TowDriverRegisteredEvent(
            publisherId.GetValue(),
            typeof(TowDriverRegistered).Name,
            new TowDriverRegistered(
                  towDriverId.GetValue()
                )
            );
        }
    }
}