using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(10)]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(30)]
        public string CompanyName { get; set; }

        [Required]
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [StringLength(20)]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(40)]
        public string ContactEmail { get; set; }

        [StringLength(20)]
        public string GSTIN { get; set; }

        [StringLength(20)]
        public string CIN { get; set; }

        [StringLength(20)]
        public string PAN { get; set; }

        [StringLength(20)]
        public string TAN { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [ForeignKey("MasterType")]
        public int? MasterTypeId { get; set; }
        public MasterType MasterType { get; set; }

        public byte? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        [ForeignKey("ModifiedByUser")]
        public int? ModifiedBy { get; set; }
        public User ModifiedByUser { get; set; }

        // Navigation Properties
        public List<Department> Departments { get; set; }
        public List<Vendor> Vendors { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Customer> Customers { get; set; }
    }
}
