using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class MasterType
    {
            [Key]
            public int MasterTypeId { get; set; }

            [Required]
            [StringLength(25)]
            public string MasterTypeName { get; set; }

            // Navigation Properties
            public List<Company> Companies { get; set; }
            public List<Department> Departments { get; set; }
            public List<Permission> Permissions { get; set; }
            public List<Document> Documents { get; set; }
        }
    }

