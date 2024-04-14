namespace CustomEd.Otp.Service.OtpService;
public interface IOtpService
{
    Task<string> GenerateOtp(Guid userId);

    Task<bool> ValidateOtp(Guid key, string otp);
}
