using System;
using System.Threading.Tasks;
using CustomEd.Contracts.Notification.Events;
using CustomEd.RTNotification.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Contracts;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;


namespace CustomEd.RTNotification.Service.Consumption.NotificationEvents
{
    public class NotifyUserEventConsumer : IConsumer<NotifyUserEvent>
    {
        private readonly IGenericRepository<Notification> _notificationRepository;
        private readonly IGenericRepository<Model.User> _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotifyUserEventConsumer(IGenericRepository<Notification> notificationRepository, IGenericRepository<Model.User> userRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task SendNotificationToGroup(NotifyUserEvent notification)
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
                await connection.InvokeAsync("SendNotificationToUser", notification);
                await connection.StopAsync();
                await connection.DisposeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task Consume(ConsumeContext<NotifyUserEvent> context)
        {
            // var notification = new Notification
            // {
            //     Description = context.Message.Description,
            //     Type = context.Message.Type,
            // };
            await SendNotificationToGroup(context.Message);
            
        }
    }
}