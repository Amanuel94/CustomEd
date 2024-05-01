using System.Net;
using CustomEd.Contracts.Otp;
using CustomEd.Otp.Service.Dtos;
using CustomEd.Otp.Service.Email.Service;
using CustomEd.Otp.Service.OtpService;
using CustomEd.Shared.Response;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEndpoint;


        public OtpController(IEmailService emailService, IOtpService otpService, IPublishEndpoint publishEndpoint)
        {
            _emailService = emailService;
            _otpService = otpService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("send")]
        public async Task<ActionResult<SharedResponse<string>>> SendOtp(
            [FromBody] SendOtpDto sendOtpDto
        )
        {
            var userId = sendOtpDto.userId;
            var email = sendOtpDto.Email;
            var role = sendOtpDto.Role;


            try
            {
                var otp = await _otpService.GenerateOtp(userId, role);
                string message =
                    $"<div style=\"color: #333333; font-family: Arial, sans-serif; font-size: 16px;\">Dear User,<br><br>"
                    + $"Your one-time password is: <span style=\"color: #ff6600; font-weight: bold;\">{otp}</span>.<br><br>"
                    + $"Please use this OTP to complete your transaction.<br><br>"
                    + $"Thank you,<br>"
                    + $"CustomEd</div>";

                await _emailService.SendEmailAsync(email, "One-Time Password", message);
                return Ok(SharedResponse<string>.Success("OTP sent successfully", null));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(SharedResponse<string>.Fail(ex.Message, null));
            }
        }

        [HttpPost("verify")]
        public async Task<ActionResult<SharedResponse<string>>> VerifyOtp(
            [FromBody] VerifyOtpDto verifyOtpDto
        )
        {
            var userId = verifyOtpDto.userId;
            var otp = verifyOtpDto.Otp;
            var role = verifyOtpDto.Role;

            try
            {
                var isValid = await _otpService.ValidateOtp(userId, otp, role);

                if (isValid)
                {
                    
                    await _publishEndpoint.Publish(new UserVerifiedEvent
                    {
                        UserId = userId,
                        Role = role
                    });

                    return Ok(SharedResponse<string>.Success("OTP verified successfully", null));
                }
                else
                {
                    return BadRequest(SharedResponse<string>.Fail("Expired/Invalid OTP", null));
                }
            }
            catch (System.Exception ex)
            {
            	 Console.WriteLine(ex.Message);
                return BadRequest(SharedResponse<string>.Fail("Failed to verify OTP", null));
            }
        }
    }
}
