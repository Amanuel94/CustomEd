using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Contracts.Notification.Events;
using CustomEd.RTNotification.Service.Model;
using Microsoft.AspNetCore.SignalR.Client;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class NotifyClassroomEventConsumer : IConsumer<NotifyClassroomEvent>
    {
        private readonly IMapper _mapper;
        public NotifyClassroomEventConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        private static async Task SendNotificationToGroup(Notification notification)
        {
            try
            {
                var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5275/notificationHub")
                .Build();
                await connection.StartAsync();
                await connection.InvokeAsync("SendNotification", notification);
                await connection.StopAsync();
                await connection.DisposeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public async Task Consume(ConsumeContext<NotifyClassroomEvent> context)
        {
            var newNotification = _mapper.Map<Notification>(context.Message);
            await SendNotificationToGroup(newNotification);
            
        }
    }
}