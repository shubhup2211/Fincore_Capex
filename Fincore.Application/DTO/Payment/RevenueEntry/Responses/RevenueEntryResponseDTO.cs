using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment.RevenueEntry.Responses
{
    public class RevenueEntryResponseDto
    {
        public int RevenueEntryId { get; set; }

        public string InvoiceNumber { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string RevenueType { get; set; }

        public decimal Amount { get; set; }

        public DateTime RevenueDate { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
