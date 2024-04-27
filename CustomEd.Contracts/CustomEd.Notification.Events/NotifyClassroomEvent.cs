using System;

namespace CustomEd.Contracts.Notification.Events;

    public class NotifyClassroomEvent
    {
        public Guid ClassroomId { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
    }
