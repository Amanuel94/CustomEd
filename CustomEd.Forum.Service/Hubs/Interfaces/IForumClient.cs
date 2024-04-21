using CustomEd.Forum.Service.Dto;
using Microsoft.AspNetCore.SignalR;

namespace CustomEd.Forum.Service.Hubs.Interfaces
{
    public interface IForumClient
    {
        Task ReceiveMessage(MessageDto message);
    }
}
