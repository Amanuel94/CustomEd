using CustomEd.Shared.Model;
using OtpNet;

namespace CustomEd.Otp.Service.Model;

public class User:BaseEntity
{
    public Guid userId {get; set;}
    public Role Role{get; set;}
}
