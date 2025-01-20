using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompanyPhoneNumberUpdatedEvent(string publisherId, string type, SupplierCompanyPhoneNumberUpdated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyPhoneNumberUpdated(string phoneNumber)
    {
        public readonly string PhoneNumber = phoneNumber;
        public static SupplierCompanyPhoneNumberUpdatedEvent CreateEvent(SupplierCompanyId publisherId, SupplierCompanyPhoneNumber phoneNumber)
        {
            return new SupplierCompanyPhoneNumberUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyPhoneNumberUpdated).Name,
                new SupplierCompanyPhoneNumberUpdated(
                    phoneNumber.GetValue()
                )
            );
        }
    }
}
