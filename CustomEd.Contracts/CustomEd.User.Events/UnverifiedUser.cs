using CustomEd.Shared.Model;

namespace CusotmEd.Contracts.User.Events;
public class UnverifiedUserEvent
    {
        public Guid Id { get; set; }
        public Role Role {get ; set; }
    }