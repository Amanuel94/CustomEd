namespace CustomEd.Forum.Service.Dto;

public class UpdateMessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid SenderId { get; set; }
    public Guid ClassroomId { get; set; }
    
}
