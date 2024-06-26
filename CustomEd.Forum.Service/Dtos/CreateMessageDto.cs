using CustomEd.Shared.Model;

namespace CustomEd.Forum.Service.Dto;

public class CreateMessageDto
{
    public string Content { get; set; } = string.Empty;
    public Guid SenderId { get; set; }
    public Role SenderRole { get; set; }
    public Guid ThreadParent { get; set; }
    public Guid ClassroomId { get; set; }
}
