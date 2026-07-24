using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces
{
    public interface ITwoFactorHelper
    {
        string GenerateSecretKey();
        string GenerateQrCodeUri(string email, string secretKey);
        bool VerifyOtp(string secretKey, string otpCode);
    }
}
