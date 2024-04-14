using System.Net;
using CustomEd.Otp.Service.Dtos;
using CustomEd.Otp.Service.Email.Service;
using CustomEd.Otp.Service.OtpService;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Mvc;
using OtpNet;

namespace CustomEd.Otp.Service.Controllers
{

    [ApiController]
    [Route("/api/one-time-verification")]
    public class OtpController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;

        public OtpController(IEmailService emailService, IOtpService otpService)
        {
            _emailService = emailService;
            _otpService = otpService;
        }

        [HttpPost("send")]
        public async Task<ActionResult<SharedResponse<string>>> SendOtp([FromBody] SendOtpDto sendOtpDto)
        {
            var userId = sendOtpDto.userId;
            var email = sendOtpDto.Email;

            try
            {
                var otp = await _otpService.GenerateOtp(userId);
                var message = $"Your one-time password is: {otp}";
    
                await _emailService.SendEmailAsync(email, "One-Time Password", message);
    
                return Ok(SharedResponse<string>.Success("OTP sent successfully", null));
            }
            catch (System.Exception)
            {
                
                return BadRequest(SharedResponse<string>.Fail("Failed to send OTP", null));
            }
        }

    [HttpPost("verify")]
        public async Task<ActionResult<SharedResponse<string>>> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var userId = verifyOtpDto.userId;
            var otp = verifyOtpDto.Otp;

            try
            {
                var isValid = await _otpService.ValidateOtp(userId, otp);

                if (isValid)
                {
                    return Ok(SharedResponse<string>.Success("OTP verified successfully", null));
                }
                else
                {
                    return BadRequest(SharedResponse<string>.Fail("Invalid OTP", null));
                }
            }
            catch (System.Exception)
            {
                return BadRequest(SharedResponse<string>.Fail("Failed to verify OTP", null));
            }
        }
    }
}

