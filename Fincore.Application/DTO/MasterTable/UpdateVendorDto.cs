using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class UpdateVendorDto
    {
        [Required]
        [StringLength(30)]
        public string VendorCode { get; set; }

        [Required]
        public int VendorCategoryId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [StringLength(30)]
        public string BankAccount { get; set; }

        [StringLength(25)]
        public string PAN { get; set; }

        [Range(0, 100)]
        public decimal? PerformanceScore { get; set; }

        public byte? IsVerified { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public int ModifiedBy { get; set; }
    }
}