using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTOs
{
    public class CurrencyRequest
    {
        public string CurrencyName { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }
}