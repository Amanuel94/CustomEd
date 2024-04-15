using CustomEd.Shared.Model;

namespace CustomEd.Otp.Service.OtpService;
public interface IOtpService
{
    Task<string> GenerateOtp(Guid userId, Role role);
    Task<bool> ValidateOtp(Guid key, string otp, Role role);
    Task<bool> IsVerified(Guid key);
}
