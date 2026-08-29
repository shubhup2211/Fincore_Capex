using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Required]
        [StringLength(30)]
        public string DepartmentName { get; set; }

        [Required]
        [StringLength(30)]
        public string DepartmentCode { get; set; }

        [ForeignKey("MasterType")]
        public int? MasterTypeId { get; set; }
        public MasterType MasterType { get; set; }

        [ForeignKey("Manager")]
        public int? ManagerId { get; set; }
        public Employee Manager { get; set; }

        [Required]
        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        [ForeignKey("ModifiedByUser")]
        public int ModifiedBy { get; set; }
        public User ModifiedByUser { get; set; }

        // Navigation Properties
        public List<Employee> Employees { get; set; }
        public List<Asset> Assets { get; set; }
        public List<BudgetCategory> BudgetCategories { get; set; }
        public List<RevenueEntry> RevenueEntries { get; set; }

        public List<BudgetLine> BudgetLines { get; set; }
    }
}
