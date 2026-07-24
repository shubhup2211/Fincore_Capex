using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class UpdateUserDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(30)]
        public string Email { get; set; }

        public string UserCategory { get; set; }

        [StringLength(12)]
        public string Phone { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public int ModifiedBy { get; set; }
    }
}