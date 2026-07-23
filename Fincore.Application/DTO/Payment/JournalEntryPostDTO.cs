using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment
{
    public class JournalEntryPostDTO
    {
        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public int AccountId { get; set; }

        public decimal? DebitAmount { get; set; }

        public decimal? CreditAmount { get; set; }

        public string? Description { get; set; }
    }
}
