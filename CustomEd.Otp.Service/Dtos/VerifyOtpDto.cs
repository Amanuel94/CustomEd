using CustomEd.Shared.Model;

namespace CustomEd.Otp.Service.Dtos
{
    public class VerifyOtpDto
    {
        public string Otp { get; set; } = null!;
        public Guid userId { get; set; }
        public Role Role { get; set; }
    }
}
