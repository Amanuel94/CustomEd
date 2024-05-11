using CustomEd.Shared.Model;

namespace CustomEd.Forum.Service.Model
{
    public class Classroom : BaseEntity
    {
        public string Name { get; set; } = null!;
        public User Creator { get; set; } = null!;
        public List<Student> Members { get; set; } = null!;
    }
}
