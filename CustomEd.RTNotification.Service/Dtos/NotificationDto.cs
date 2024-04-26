using CustomEd.Shared.Model;

namespace CustomEd.RTNotification.Service.Dto;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public List<Guid> Receivers { get; set; } = new();
    public Guid ClassroomId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}