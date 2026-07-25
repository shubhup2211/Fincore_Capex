using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTOs
{
    public class CountryRequestDto
    {
        public int CountryCode { get; set; }

        public string CountryName { get; set; } = string.Empty;

        public int CurrencyId { get; set; }
    }
}