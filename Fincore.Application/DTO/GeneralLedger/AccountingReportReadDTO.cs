using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.GeneralLedger
{
     public class AccountingReportReadDTO
    {

        public string JournalNumber { get; set; }

        public DateTime EntryDate { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }

        public string? Description { get; set; }
    }
}
