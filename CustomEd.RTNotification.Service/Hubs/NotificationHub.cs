using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using CustomEd.RTNotification.Service.Dto;
using CustomEd.RTNotification.Service.Hubs.Interfaces;
using CustomEd.RTNotification.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CustomEd.RTNotification.Service.Hubs;

public class NotificationHub : Hub<INotifcationClient>
{
    private readonly ConcurrentDictionary<Guid, string> _connections =
        new ConcurrentDictionary<Guid, string>();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<Model.User> _userRepository;
    private readonly IGenericRepository<Model.Classroom> _classroomRepository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;

    public NotificationHub(
        ConcurrentDictionary<Guid, string> connections,
        IHttpContextAccessor httpContextAccessor,
        IGenericRepository<Model.User> userRepository,
        IGenericRepository<Classroom> classroomRepository,
        IMapper mapper,
        IJwtService jwtService
    )
    {
        _connections = connections;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _classroomRepository = classroomRepository;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var token = Context.GetHttpContext()!.Request.Query["token"];

            if (string.IsNullOrEmpty(token) || _jwtService.IsTokenValid(token!) == false)
            {
                throw new Exception("Invalid token");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);
            var userId = Guid.Parse(jwt.Claims.First(x => x.Type == "nameid").Value);


            var user = await _userRepository.GetAsync(userId);

            if (user == null)
            {
                Console.WriteLine("User not found");
                throw new Exception("User not found");
            }
            _connections.TryAdd(userId, Context.ConnectionId);

            var groups = await _classroomRepository.GetAllAsync(x =>
                x.Members.Select(m => m.Id).Contains(userId) || x.Creator.Id == userId
            );

            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            }

            foreach (var message in user.UnreadNotifications)
            {
                await Clients.Caller.ReceiveNotification(_mapper.Map<NotificationDto>(message));
                user.UnreadNotifications.Remove(message);
                await _userRepository.UpdateAsync(user);
            }
            




        }
        catch (System.Exception e)
        {
            // ignored
            Console.WriteLine(e.StackTrace);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception) {
        try
        {
            var token = Context.GetHttpContext()!.Request.Query["token"];

            if (string.IsNullOrEmpty(token) || _jwtService.IsTokenValid(token!) == false)
            {
                throw new Exception("Invalid token");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);
            var userId = Guid.Parse(jwt.Claims.First(x => x.Type == "nameid").Value);

            _connections.TryRemove(userId, out _);

            var groups = await _classroomRepository.GetAllAsync(x =>
                x.Members.Select(m => m.Id).Contains(userId) || x.Creator.Id == userId
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

    public async Task SendNotification(Notification notification) { }
}
