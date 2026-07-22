using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(30)]
        public string RoleName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; }

        public byte? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        [ForeignKey("ModifiedByUser")]
        public int? ModifiedBy { get; set; }
        public User? ModifiedByUser { get; set; }

        // Navigation Properties
        public List<User> Users { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<ApprovalFlow> ApprovalFlows { get; set; }
    }
}
