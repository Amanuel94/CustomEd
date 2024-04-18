namespace CustomEd.Forum.Service.Dto;

public class MessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public UserDto Sender { get; set; } = null!;
    public Guid ClassroomId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}
