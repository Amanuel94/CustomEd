using CustomEd.Shared.Model;

namespace CustomEd.Forum.Service.Model;

public class User : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!; 
    public string PhoneNumber { get; set; } = null!;    
    public Role Role { get; set; }
    public List<Message> UnreadMessages { get; set; } = null!;
}
