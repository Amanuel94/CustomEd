using CustomEd.RTNotification.Service.Dto;
using Microsoft.AspNetCore.SignalR;

namespace CustomEd.RTNotification.Service.Hubs.Interfaces
{
    public interface INotifcationClient
    {
        Task ReceiveNotification(NotificationDto message);
    }
}
