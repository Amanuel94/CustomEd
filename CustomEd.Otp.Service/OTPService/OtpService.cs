using CustomEd.Shared.Data.Interfaces;
using OtpNet;
using CustomEd.Otp.Service.Model;

namespace CustomEd.Otp.Service.OtpService;
public class OtpService : IOtpService
{
    private IGenericRepository<Model.Otp> _otpRepository;

    public OtpService(IGenericRepository<Model.Otp> otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public  async Task<string> GenerateOtp(Guid userId)
    {
        var otpKey = KeyGeneration.GenerateRandomKey(20); 
        var hotp = new Hotp(otpKey);
        var otp = hotp.ComputeHOTP(0); 
        var prevOtps = await _otpRepository.GetAllAsync(u => u.userId == userId);
        foreach (var prevOtp in prevOtps)
        {
            await _otpRepository.RemoveAsync(prevOtp);
        }
        await _otpRepository.CreateAsync(new Model.Otp {userId = userId, otp = hotp, Expiration = DateTime.Now.AddMinutes(5)});
        return otp;
    }

    public async Task<bool> ValidateOtp(Guid key, string otp)
    {
        var otpEntity = await _otpRepository.GetAsync(o => o.otp.ComputeHOTP(0) == otp && o.userId == key);

        if (otpEntity == null)
        {
            return false;
        }

        if (DateTime.Now > otpEntity.Expiration)
        {
            await _otpRepository.RemoveAsync(otpEntity);
            return false;
        }

        await _otpRepository.RemoveAsync(otpEntity);
        return true;
    }
}
