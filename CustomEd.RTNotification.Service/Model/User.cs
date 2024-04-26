using CustomEd.RTNotification.Service.Dto;
using CustomEd.Shared.Model;

namespace CustomEd.RTNotification.Service.Model;

public class User : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!; 
    public string PhoneNumber { get; set; } = null!;    
    public Role Role { get; set; }
    public List<Notification> UnreadNotifications { get; set; } = null!;
    
    public string Key 
    {
        get 
        {
            return $"{Id}-{Role}";  
        }
    }
}
