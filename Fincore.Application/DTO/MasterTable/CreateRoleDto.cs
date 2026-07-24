using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(30)]
        public string RoleName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public byte? IsActive { get; set; }

        [Required]
        public int CreatedBy { get; set; }
    }
}