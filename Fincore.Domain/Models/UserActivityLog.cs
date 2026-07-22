using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class UserActivityLog
    {
        [Key]
        public long UserActivityLogId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(100)]
        public string ActivityType { get; set; }

        [StringLength(100)]
        public string Module { get; set; }

        [Required]
        public DateTime ActivityDate { get; set; }
    }
}
