using CustomEd.Shared.Data.Interfaces;
using OtpNet;
using CustomEd.Otp.Service.Model;
using CustomEd.Shared.Model;
using CustomEd.Otp.Service.Errors;

namespace CustomEd.Otp.Service.OtpService;
public class OtpService : IOtpService
{
    private IGenericRepository<Model.Otp> _otpRepository;
    private readonly IGenericRepository<Model.User> _userRepository;


    public OtpService(IGenericRepository<Model.Otp> otpRepository, IGenericRepository<Model.User> userRepository)
    {
        _otpRepository = otpRepository;
        _userRepository = userRepository;
        
    }

    public  async Task<string> GenerateOtp(Guid userId, Role role)
    {
        if(await _userRepository.GetAsync(u => u.userId == userId && u.Role == role) == null)
        {
            throw new NonUserException();
        }

        var otpKey = KeyGeneration.GenerateRandomKey(20); 
        var hotp = new Hotp(otpKey);
        var otp = hotp.ComputeHOTP(0); 
        var prevOtps = await _otpRepository.GetAllAsync(u => u.userId == userId && u.Role == role);
        foreach (var prevOtp in prevOtps)
        {
            await _otpRepository.RemoveAsync(prevOtp);
        }
        await _otpRepository.CreateAsync(new Model.Otp {userId = userId, otp = otp, Expiration = DateTime.UtcNow.AddMinutes(10), Role = role});
        return otp;
    }

    public async Task<bool> ValidateOtp(Guid key, string otp, Role role)
    {
        

        var otpEntity = await _otpRepository.GetAsync(o => o.otp == otp && o.userId == key && o.Role == role);

        if (otpEntity == null)
        {
            Console.WriteLine("OTP is null");
            return false;
        }

        if (DateTime.UtcNow > otpEntity.Expiration)
        {
            Console.WriteLine("OTP is expired");
            await _otpRepository.RemoveAsync(otpEntity);
            return false;
        }



        await _otpRepository.RemoveAsync(otpEntity);
        return true;
    }

    public  Task<bool> IsVerified(Guid key)
    {
        throw new NotImplementedException();
    }
}
