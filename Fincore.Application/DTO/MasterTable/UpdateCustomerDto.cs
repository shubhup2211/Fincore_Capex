using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class UpdateCustomerDto
    {
        [Required]
        [StringLength(30)]
        public string CustomerCode { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public byte IsActive { get; set; }
    }
}