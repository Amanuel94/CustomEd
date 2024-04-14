using CustomEd.Shared.Model;
using OtpNet;

namespace CustomEd.Otp.Service.Model;

public class Otp:BaseEntity
{
    public Hotp otp { get; set; } = null!;
    public Guid userId {get; set;}
    public DateTime Expiration {get; set;}
}
