using CustomEd.Shared.Model;

namespace CustomEd.Contracts.Otp;
    public class UserVerifiedEvent
    {
        public Guid UserId { get; set; }
        public Role Role { get; set; }

    }

