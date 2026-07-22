using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        [Required]
        [ForeignKey("DesignationRole")]
        public int Designation { get; set; }
        public Role DesignationRole { get; set; }

        public DateTime? JoiningDate { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Column("Reporting Manager")]
        [ForeignKey("ReportingManagerEmployee")]
        public int? ReportingManager { get; set; }
        public Employee ReportingManagerEmployee { get; set; }

        [StringLength(25)]
        public string PAN { get; set; }

        [Required]
        public byte IsActive { get; set; }

        // Navigation Properties
        public List<Employee> Subordinates { get; set; }
        public List<RFQ> RFQsCreated { get; set; }
        public List<GRN> GRNsCreated { get; set; }
    }
}
