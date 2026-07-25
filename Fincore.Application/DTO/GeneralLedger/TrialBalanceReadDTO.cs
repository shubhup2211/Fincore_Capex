using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.GeneralLedger
{
     public class TrialBalanceReadDTO
    {
        public int AccountId { get; set; }

        public string AccountCode { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;

        public decimal TotalDebit { get; set; }

        public decimal TotalCredit { get; set; }
    }
}
