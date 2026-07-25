using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fincore.Domain.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        //[StringLength(5)]
        public int CountryCode { get; set; }

        [Required]
        [StringLength(30)]
        public string CountryName { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        // Navigation Properties
        public Currency? Currency { get; set; }

        public List<Company> Companies { get; set; } = new();

        public List<State> States { get; set; } = new();
    }
}