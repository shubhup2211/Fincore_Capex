using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class AccountMaster
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        [StringLength(30)]
        public string AccountCode { get; set; }

        [Required]
        [StringLength(60)]
        public string AccountName { get; set; }

        [Required]
        [StringLength(30)]
        public string AccountType { get; set; }

        [Required]
        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        [ForeignKey("ModifiedByUser")]
        public int ModifiedBy { get; set; }
        public User ModifiedByUser { get; set; }

        // Navigation Properties
        public List<RevenueEntry> RevenueEntries { get; set; }
        public List<JournalEntry> JournalEntries { get; set; }
    }
}
