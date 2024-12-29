using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record CreatePolicyDto(
        [Required]
        string SupplierCompanyId,
        [Required]
        string Title,
        [Required]
        int CoverageAmount,
        [Required]
        decimal Price,
        [Required]
        string Type,
        [Required]
        DateTime IssuanceDate,
        [Required]
        DateTime ExpirationDate
    );
}