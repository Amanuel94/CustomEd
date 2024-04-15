using CustomEd.Shared.Model;

namespace CustomEd.Otp.Service.Dtos
{
    public class SendOtpDto
    {
        public string Email { get; set; } = null!;
        public Guid userId { get; set; }
        public Role Role { get; set; }
    }
}
