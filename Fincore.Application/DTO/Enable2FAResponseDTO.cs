using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO
{
    public class Enable2FAResponseDTO
    {
        public string SecretKey { get; set; }
        public string QrCodeUri { get; set; }
    }
}

