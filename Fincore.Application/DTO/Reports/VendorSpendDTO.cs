using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
    public class VendorSpendDTO
    {
        public int PaymentId { get; set; }

        public string PaymentNumber { get; set; }

        public string VendorCode { get; set; }

        public string CompanyName { get; set; }

        public decimal Amount { get; set; }

        public string APInvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; }

        public string ApprovalStatus { get; set; }
    }
}
