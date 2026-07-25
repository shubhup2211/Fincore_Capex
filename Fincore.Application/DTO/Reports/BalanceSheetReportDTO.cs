using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
     public class BalanceSheetReportDTO
    {
        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string AccountType { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }

        public decimal Balance { get; set; }
    }
}
