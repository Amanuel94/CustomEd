using CustomEd.Shared.Model;

namespace CustomEd.RTNotification.Service.Model;

public class Notification: BaseEntity
{
    public string Description { get; set; } = null!;
    public string Type { get; set; } = null!;
    public Guid ClassroomId { get; set; }
    
}