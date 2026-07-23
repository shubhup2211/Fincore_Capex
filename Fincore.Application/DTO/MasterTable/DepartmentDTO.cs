using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }

        [Required]
        [StringLength(30)]
        public string DepartmentName { get; set; }

        [Required]
        [StringLength(30)]
        public string DepartmentCode { get; set; }

        public int? MasterTypeId { get; set; }

        public string? MasterTypeName { get; set; }

        public int? ManagerId { get; set; }

        public string? ManagerName { get; set; }

        [Required]
        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int ModifiedBy { get; set; }
    }
}