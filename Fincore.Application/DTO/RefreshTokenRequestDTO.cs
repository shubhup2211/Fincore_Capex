using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO
{
    public class RefreshTokenRequestDTO
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
