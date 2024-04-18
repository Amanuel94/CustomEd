using CustomEd.Shared.Model;

namespace CustomEd.Forum.Service.Model;

public class Message : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public User Sender { get; set; } = null!;
    public Classroom? Classroom { get; set; }
    
}
