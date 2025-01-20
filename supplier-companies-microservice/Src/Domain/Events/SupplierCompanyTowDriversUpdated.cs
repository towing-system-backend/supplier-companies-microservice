using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyTowDriversUpdatedEvent(string publisherId, string type, SupplierCompanyTowDriversUpdated context) : DomainEvent(publisherId, type, context) { }
    public class SupplierCompanyTowDriversUpdated(List<string> towDrivers)
    {
        public readonly List<string> TowDrivers = towDrivers;

        public static SupplierCompanyTowDriversUpdatedEvent CreateEvent(SupplierCompanyId publisherId, List<TowDriverId> towDrivers)
        {
            return new SupplierCompanyTowDriversUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyTowDriversUpdated).Name,
                new SupplierCompanyTowDriversUpdated(
                    towDrivers.Select(td => td.GetValue()).ToList()
                )
            );
        }
    }
}