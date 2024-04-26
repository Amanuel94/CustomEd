using CustomEd.Shared.Model;

namespace CustomEd.RTNotification.Service.Model
{
    public class Classroom : BaseEntity
    {
        public string Name { get; set; } = null!;
        public User Creator { get; set; } = null!;
        public List<User> Members { get; set; } = null!;
    }
}
