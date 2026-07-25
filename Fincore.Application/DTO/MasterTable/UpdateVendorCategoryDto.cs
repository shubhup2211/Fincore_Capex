using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class UpdateVendorCategoryDto
    {
        [Required]
        [StringLength(30)]
        public string CategoryName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public int ModifiedBy { get; set; }
    }
}