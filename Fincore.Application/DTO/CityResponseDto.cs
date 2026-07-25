using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTOs
{
    public class CityResponseDto
    {
        public int CityId { get; set; }

        public string CityName { get; set; } = string.Empty;

        public int StateId { get; set; }
    }
}