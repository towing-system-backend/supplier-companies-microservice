using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record RegisterTowDriverDto(
        [Required]
        string SupplierCompanyId,
        [Required]
        string Id
    );
}
