using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class CapexReqDTOPost
    {
        public int CapexRequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int DepartmentId { get; set; }
        public int BudgetLineId { get; set; }
        public int RequestedBy { get; set; }

    }
}
