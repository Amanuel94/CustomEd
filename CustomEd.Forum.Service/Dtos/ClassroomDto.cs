
namespace CustomEd.Forum.Service.Dto
{
    public class ClassroomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public UserDto Creator { get; set; } = null!;
        public List<UserDto> Members { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
