using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class JournalEntry
    {
        [Key]
        public int JournalEntryId { get; set; }

        [Required]
        [StringLength(50)]
        public string JournalNumber { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        [ForeignKey("AccountMaster")]
        public int AccountId { get; set; }
        public AccountMaster AccountMaster { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DebitAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditAmount { get; set; }

        public string Description { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
