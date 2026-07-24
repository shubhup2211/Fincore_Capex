using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Application.Interfaces;
using OtpNet;

namespace Fincore.Infrastructure
{
    public class TwoFactorHelper : ITwoFactorHelper
    {
        public string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeUri(string email, string secretKey)
        {
            string issuer = "FinCoreERP";
            return $"otpauth://totp/{issuer}:{email}?secret={secretKey}&issuer={issuer}&digits=6";
        }

        public bool VerifyOtp(string secretKey, string otpCode)
        {
            var bytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(bytes);
            return totp.VerifyTotp(otpCode, out _, new VerificationWindow(previous: 1, future: 1));
        }
    }
}
