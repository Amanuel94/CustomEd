using CustomEd.Shared.Data.Interfaces;
using OtpNet;
using CustomEd.Otp.Service.Model;
using CustomEd.Shared.Model;

namespace CustomEd.Otp.Service.OtpService;
public class OtpService : IOtpService
{
    private IGenericRepository<Model.Otp> _otpRepository;
    private readonly IGenericRepository<VerifiedUser> _verifiedUserRepository;

    public OtpService(IGenericRepository<Model.Otp> otpRepository, IGenericRepository<VerifiedUser> verifiedUserRepository)
    {
        _otpRepository = otpRepository;
        _verifiedUserRepository = verifiedUserRepository;
    }

    public  async Task<string> GenerateOtp(Guid userId, Role role)
    {
        var otpKey = KeyGeneration.GenerateRandomKey(20); 
        var hotp = new Hotp(otpKey);
        var otp = hotp.ComputeHOTP(0); 
        var prevOtps = await _otpRepository.GetAllAsync(u => u.userId == userId && u.Role == role);
        foreach (var prevOtp in prevOtps)
        {
            await _otpRepository.RemoveAsync(prevOtp);
        }
        await _otpRepository.CreateAsync(new Model.Otp {userId = userId, otp = hotp, Expiration = DateTime.Now.AddMinutes(5)});
        return otp;
    }

    public async Task<bool> ValidateOtp(Guid key, string otp, Role role)
    {
        var otpEntity = await _otpRepository.GetAsync(o => o.otp.ComputeHOTP(0) == otp && o.userId == key && o.Role == role);

        if (otpEntity == null)
        {
            return false;
        }

        if (DateTime.Now > otpEntity.Expiration)
        {
            await _otpRepository.RemoveAsync(otpEntity);
            return false;
        }



        await _verifiedUserRepository.CreateAsync(new VerifiedUser {Id = key});        
        await _otpRepository.RemoveAsync(otpEntity);
        return true;
    }

    public async Task<bool> IsVerified(Guid key)
    {
        return await _verifiedUserRepository.GetAsync(u => u.Id == key) != null;
    }
}
