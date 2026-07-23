using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment
{
    public class JournalEntryGetDTO
    {
        public int JournalEntryId { get; set; }

        public string JournalNumber { get; set; }

        public DateTime EntryDate { get; set; }

        public int AccountId { get; set; }

        public decimal? DebitAmount { get; set; }

        public decimal? CreditAmount { get; set; }

        public string? Description { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
