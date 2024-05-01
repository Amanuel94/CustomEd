using CustomEd.Shared.Model;
using OtpNet;

namespace CustomEd.Otp.Service.Model;

public class Otp : BaseEntity
{
    public string otp { get; set; } = null!;
    public Guid userId { get; set; }
    public Role Role { get; set; }
    public DateTime Expiration { get; set; }
}
