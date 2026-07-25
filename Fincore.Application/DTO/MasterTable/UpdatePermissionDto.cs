using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class UpdatePermissionDto
    {
        [Required]
        [StringLength(50)]
        public string PermissionName { get; set; }

        [Required]
        public int RoleId { get; set; }

        public int? MasterTypeId { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public int ModifiedBy { get; set; }
    }
}