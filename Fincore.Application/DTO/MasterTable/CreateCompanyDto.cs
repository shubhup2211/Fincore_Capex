using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.MasterTable
{
    public class CreateCompanyDto
    {
        [Required]
        [StringLength(30)]
        public string CompanyName { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Required]
        [StringLength(20)]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(40)]
        public string ContactEmail { get; set; }

        [Required]
        [StringLength(20)]
        public string GSTIN { get; set; }

        [Required]
        [StringLength(20)]
        public string CIN { get; set; }

        [Required]
        [StringLength(20)]
        public string PAN { get; set; }

        [Required]
        [StringLength(20)]
        public string TAN { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        public int? MasterTypeId { get; set; }
    }
}
