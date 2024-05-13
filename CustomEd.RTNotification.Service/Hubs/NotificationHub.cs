using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using CustomEd.Contracts.Notification.Events;
using CustomEd.RTNotification.Service.Dto;
using CustomEd.RTNotification.Service.Hubs.Interfaces;
using CustomEd.RTNotification.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using Microsoft.AspNetCore.SignalR;

namespace CustomEd.RTNotification.Service.Hubs;

public class NotificationHub : Hub<INotifcationClient>
{
    private readonly ConcurrentDictionary<Guid, string> _connections =
        new ConcurrentDictionary<Guid, string>();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<Model.User> _userRepository;
    private readonly IGenericRepository<Model.Classroom> _classroomRepository;
    private readonly IGenericRepository<Model.Notification> _notificationRepository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private Guid _adminId { get; set; } = Guid.Empty;

    public NotificationHub(
        ConcurrentDictionary<Guid, string> connections,
        IHttpContextAccessor httpContextAccessor,
        IGenericRepository<Model.User> userRepository,
        IGenericRepository<Classroom> classroomRepository,
        IGenericRepository<Notification> notificationRepository,
        IMapper mapper,
        IJwtService jwtService
    )
    {
        _connections = connections;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _classroomRepository = classroomRepository;
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"ConnectionId {Context.ConnectionId} is connecting. . .");
        try
        {
            // var token = Context.GetHttpContext().Request.Headers["Authorization"];
            // if (string.IsNullOrEmpty(token))
            // {
            //     Console.WriteLine("No token found");
            //     return;
            // }
            // Console.WriteLine(token);
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
            var user = await _userRepository.GetAsync(currentUserId);
            if (userRole != Role.Admin && user == null)
            {
                return;
            }
            if(userRole == Role.Admin) _adminId = currentUserId;

            _connections.TryAdd(currentUserId, Context.ConnectionId);

            var groups = await _classroomRepository.GetAllAsync(x =>
                x.Members.Select(m => m.Id).Contains(currentUserId) || x.Creator.Id == currentUserId
            );

            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            }
            Console.WriteLine("Successful connection");
        }
        catch (System.Exception e)
        {
            // ignored
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            _connections.TryRemove(currentUserId, out _);
            var groups = await _classroomRepository.GetAllAsync(x =>
                x.Members.Select(m => m.Id).Contains(currentUserId) || x.Creator.Id == currentUserId
            );

            foreach (var group in groups)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Id.ToString());
            }
        }
        catch (System.Exception e)
        {
            // ignored
            Console.WriteLine(e.StackTrace);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotification(Notification notification)
    {
        await _notificationRepository.CreateAsync(notification);
        var group = await _classroomRepository.GetAsync(notification.ClassroomId);
        if(_connections.Keys.Contains<Guid>(_adminId) && Context.ConnectionId != _connections[_adminId])
        {
            return;
        }
        await Clients
            .Group(notification.ClassroomId.ToString())
            .ReceiveNotification(_mapper.Map<NotificationDto>(notification));
        foreach (var member in group.Members)
        {
            if (!_connections.Keys.Contains(member.Id))
            {
                Console.WriteLine("Saving notification to member");
                Console.WriteLine(member.Id);
                Console.WriteLine(notification.Description);
                var user = await _userRepository.GetAsync(member.Id);
                if (user.UnreadNotifications == null)
                {
                    user.UnreadNotifications = new List<Notification>()!;
                }
                user.UnreadNotifications.Add(notification);
                await _userRepository.UpdateAsync(user);
            }
        }
        if (!_connections.Keys.Contains(group.Creator.Id))
        {
            var user = await _userRepository.GetAsync(group.Creator.Id);
            Console.WriteLine("Saving notification to creator");
            Console.WriteLine(user.Id);
            Console.WriteLine(notification.Description);
            if (user.UnreadNotifications == null)
            {
                user.UnreadNotifications = new List<Notification>()!;
            }
            user.UnreadNotifications.Add(notification);
            await _userRepository.UpdateAsync(user);
        }
    }
    
    public async Task SendNotificationToUser(NotifyUserEvent notification)
    {
        var user = await _userRepository.GetAsync(notification.ReceiverId);
        if (user == null)
        {
            return;
        }
        var newNotification = new Notification
        {
            Description = notification.Description,
            Type = notification.Type,
            
        };
        await _notificationRepository.CreateAsync(newNotification);
        if (_connections.Keys.Contains(notification.ReceiverId))
        {
            await Clients
                .Client(_connections[notification.ReceiverId])
                .ReceiveNotification(_mapper.Map<NotificationDto>(newNotification));
        }
        else
        {
            if (user.UnreadNotifications == null)
            {
                user.UnreadNotifications = new List<Notification>()!;
            }
            user.UnreadNotifications.Add(newNotification);
            await _userRepository.UpdateAsync(user);
        }
    }

}
