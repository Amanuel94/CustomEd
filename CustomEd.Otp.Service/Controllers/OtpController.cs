/*using System.Net;
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
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            var otp = _otpService.GenerateOtp();
            var message = $"Your one-time password is: {otp}";

            await _emailService.SendEmailAsync(email, "One-Time Password", message);

            return Ok(new ApiResponse<string> { Result = otp });
        }
    }
}
*/
