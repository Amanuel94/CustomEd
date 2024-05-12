using MassTransit;
using AutoMapper;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Contracts.Notification.Events;
using CustomEd.RTNotification.Service.Model;
using Microsoft.AspNetCore.SignalR.Client;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.Model;

namespace CustomEd.RTNotification.Service.Consumers
{
    public class NotifyClassroomEventConsumer : IConsumer<NotifyClassroomEvent>
    {
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
       
        public NotifyClassroomEventConsumer(IMapper mapper, IJwtService jwtService)
        {
            _mapper = mapper;
            _jwtService = jwtService;
        }
        
        private async Task SendNotificationToGroup(Notification notification)
        {
            try
            {
                var admin = new UserDto{
                    Id = Guid.Empty,
                    Email = "",
                    Role = (IdentityRole)Role.Admin
                };
                var token = _jwtService.GenerateToken(admin);
                var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5275" + "/notificationhub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token)!;
                })
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