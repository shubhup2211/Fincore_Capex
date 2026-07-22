using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
    public class RevenueReportDTO
    {
        public int RevenueEntryId { get; set; }

        public string InvoiceNumber { get; set; }

        public string CustomerCode { get; set; }

        public string DepartmentName { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string RevenueType { get; set; }

        public decimal Amount { get; set; }

        public DateTime RevenueDate { get; set; }

        public string Status { get; set; }
    }
}
