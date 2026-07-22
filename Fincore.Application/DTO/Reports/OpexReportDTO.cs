using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
    public class OpexReportDTO
    {
        public int OpexRequestId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public string ApprovalStatus { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}
