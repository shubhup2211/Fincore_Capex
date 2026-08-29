using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class CapexReqDTOGet
    {
            public int CapexRequestId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public string Department { get; set; }
            public int BudgetLineId { get; set; }
            public string RequestedBy { get; set; }
            public string ApprovalStatus { get; set; }
            public string? ApprovedBy { get; set; }
            public DateTime? ApprovedAt { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? ModifiedAt { get; set; }

    }
}
