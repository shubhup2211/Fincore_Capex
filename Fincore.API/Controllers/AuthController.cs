using Fincore.Application.DTO;
using Fincore.Application.Interfaces;
using Fincore.Infrastructure.CommonHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("api/v1/auth/register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            try
            {
                var msg = await authService.Register(dto);
                return Ok(ApiResponseHelper.SuccessRes(msg, "User registered successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<string>("Registration failed", "REGISTER_ERROR", ex.Message));
            }
        }

        [HttpPost]
        [Route("api/v1/auth/login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                var result = await authService.Login(dto);
                return Ok(ApiResponseHelper.SuccessRes(result, "Login processed."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<LoginResponseDTO>("Login failed", "LOGIN_ERROR", ex.Message));
            }
        }

        [HttpPost]
        [Route("api/v1/auth/verify-otp")]
        public async Task<IActionResult> VerifyOtp(Verify2FADTO dto)
        {
            try
            {
                var result = await authService.VerifyOtpAndLogin(dto);
                return Ok(ApiResponseHelper.SuccessRes(result, "OTP verified. Login successful."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<LoginResponseDTO>("OTP verification failed", "OTP_ERROR", ex.Message));
            }
        }

        [HttpPost]
        [Route("api/v1/auth/refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDTO dto)
        {
            try
            {
                var result = await authService.RefreshToken(dto);
                return Ok(ApiResponseHelper.SuccessRes(result, "Token refreshed."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<LoginResponseDTO>("Refresh failed", "REFRESH_ERROR", ex.Message));
            }
        }

        // Logged-in user hi apna 2FA enable kar sake -> [Authorize] laga hai
        [Authorize]
        [HttpPost]
        [Route("api/v1/auth/enable-2fa")]
        public async Task<IActionResult> Enable2FA(Enable2FARequestDTO dto)
        {
            try
            {
                var result = await authService.GenerateTwoFactorSetup(dto.UserId);
                return Ok(ApiResponseHelper.SuccessRes(result, "Scan this QR in Google Authenticator app."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<Enable2FAResponseDTO>("2FA setup failed", "2FA_SETUP_ERROR", ex.Message));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/auth/confirm-2fa")]
        public async Task<IActionResult> Confirm2FA(Confirm2FARequestDTO dto)
        {
            try
            {
                var msg = await authService.ConfirmTwoFactorSetup(dto);
                return Ok(ApiResponseHelper.SuccessRes(msg, "2FA confirmed."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<string>("2FA confirmation failed", "2FA_CONFIRM_ERROR", ex.Message));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/v1/auth/disable-2fa")]
        public async Task<IActionResult> Disable2FA(int userId)
        {
            try
            {
                var msg = await authService.DisableTwoFactor(userId);
                return Ok(ApiResponseHelper.SuccessRes(msg, "2FA disabled."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<string>("Disable 2FA failed", "2FA_DISABLE_ERROR", ex.Message));
            }
        }
    }
}
