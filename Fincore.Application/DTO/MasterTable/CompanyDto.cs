using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.MasterTable
{
    public class CompanyDto
    {
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        public int CountryId { get; set; }

        // Country name for display
        public string CountryName { get; set; }

        public string ContactNumber { get; set; }

        public string ContactEmail { get; set; }

        public string GSTIN { get; set; }

        public string CIN { get; set; }

        public string PAN { get; set; }

        public string TAN { get; set; }

        public string Address { get; set; }

        public int? MasterTypeId { get; set; }

        // Master type name for display
        public string MasterTypeName { get; set; }

        public byte? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
