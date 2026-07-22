using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        [StringLength(30)]
        public string CityName { get; set; }

        [Required]
        [ForeignKey("State")]
        public int StateId { get; set; }
        public State State { get; set; }
    }
}
