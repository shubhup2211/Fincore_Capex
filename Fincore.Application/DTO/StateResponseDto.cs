using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTOs
{
    public class StateResponseDto
    {
        public int StateId { get; set; }

        public string StateName { get; set; } = string.Empty;

        public int CountryId { get; set; }
    }
}