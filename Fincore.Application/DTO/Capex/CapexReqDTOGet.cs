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
            public int DepartmentId { get; set; }
            public int BudgetLineId { get; set; }
            public int RequestedBy { get; set; }
            public string ApprovalStatus { get; set; }
            public int? ApprovedBy { get; set; }
            public DateTime? ApprovedAt { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? ModifiedAt { get; set; }

    }
}
