using CustomEd.Shared.Model;

namespace CustomEd.Contracts.Notification.Events;

public class NotifyUserEvent : BaseEntity
{
    public Guid ReceiverId { get; set; }
    public string Description { get; set; } = null!;
    public string Type { get; set; } = null!;
    public Role role { get; set; }
    
}
