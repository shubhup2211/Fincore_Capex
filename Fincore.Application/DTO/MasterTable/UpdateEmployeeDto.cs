using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.MasterTable
{
    public class UpdateEmployeeDto
    {
        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int Designation { get; set; }

        public DateTime? JoiningDate { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public int? ReportingManager { get; set; }

        [StringLength(25)]
        public string PAN { get; set; }

        [Required]
        public byte IsActive { get; set; }
    }
}