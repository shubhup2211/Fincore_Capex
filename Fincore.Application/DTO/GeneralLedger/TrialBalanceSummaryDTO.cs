using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.GeneralLedger
{
    public class TrialBalanceSummaryDTO
    {
        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public bool IsBalanced { get; set; }
    }
}
