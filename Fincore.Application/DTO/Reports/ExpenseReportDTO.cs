using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
     public class ExpenseReportDTO
    {
        public int ExpenseClaimId { get; set; }

        public string ClaimNumber { get; set; }

        public string Description { get; set; }

        public string ExpenseType { get; set; }

        public decimal ExpenseAmount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string ApprovalStatus { get; set; }

        public string OpexTitle { get; set; }

        public string ClaimedBy { get; set; }
    }
}
