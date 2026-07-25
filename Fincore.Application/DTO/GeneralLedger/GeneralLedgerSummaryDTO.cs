using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.GeneralLedger
{
     public class GeneralLedgerSummaryDTO
    {
        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }

        public int TotalTransactions { get; set; }
    }
}
