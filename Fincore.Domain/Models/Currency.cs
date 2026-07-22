using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Metrics;
        public class Currency
        {
            [Key]
            public int CurrencyId { get; set; }

            [Required]
            [StringLength(20)]
            public string CurrencyName { get; set; }

            [StringLength(5)]
            public string Symbol { get; set; }

            // Navigation Properties
            public List<Country> Countries { get; set; }
        }
}
