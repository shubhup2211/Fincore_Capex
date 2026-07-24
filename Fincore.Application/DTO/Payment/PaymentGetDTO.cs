using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment
{
    public class PaymentGetDTO
    {
        public int PaymentId { get; set; }

        public string PaymentNumber { get; set; }

        public string PaymentType { get; set; }

        public int? ARInvoiceId { get; set; }

        public int? APInvoiceId { get; set; }

        public int? VendorId { get; set; }

        public int? CustomerId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; }

        public int? ApprovedBy { get; set; }

        public bool? ReconciledFlag { get; set; }

        public string ApprovalStatus { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }


    }
}
