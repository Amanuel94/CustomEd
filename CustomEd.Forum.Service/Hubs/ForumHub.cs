using System.Collections.Concurrent;
using System.Threading.Tasks;
using AutoMapper;
using CustomEd.Forum.Service.Dto;
using CustomEd.Forum.Service.Hubs.Interfaces;
using CustomEd.Forum.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.SignalR;

public class ForumHub : Hub<IForumClient>
{
    private readonly ConcurrentDictionary<Guid, string> _connections =
        new ConcurrentDictionary<Guid, string>();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<Classroom> _classroomRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;

    public ForumHub(
        ConcurrentDictionary<Guid, string> connections,
        IHttpContextAccessor httpContextAccessor,
        IGenericRepository<Classroom> classroomRepository,
        IGenericRepository<User> userRepository,
        IMapper mapper,
        IJwtService jwtService
    )
    {
        _connections = connections;
        _httpContextAccessor = httpContextAccessor;
        _classroomRepository = classroomRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();

            var user = await _userRepository.GetAsync(currentUserId);

            _connections.TryAdd(currentUserId, Context.ConnectionId);

            var groups = await _classroomRepository.GetAllAsync(x =>
                x.Members.Select(m => m.Id).Contains(currentUserId) || x.Creator.Id == currentUserId
            );

            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            }

            foreach (var message in user.UnreadMessages)
            {
                await Clients.Caller.ReceiveMessage(_mapper.Map<MessageDto>(message));
                user.UnreadMessages.Remove(message);
                await _userRepository.UpdateAsync(user);
            }
        }
        catch (System.Exception) { 
            // ignored
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        try
        {
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            _connections.TryRemove(userId, out _);
            var groups = await _classroomRepository.GetAllAsync(x =>
                x.Members.Select(m => m.Id).Contains(userId) || x.Creator.Id == userId
            );
            foreach (var group in groups)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Id.ToString());
            }
        }
        catch (System.Exception)
        {
            
            // ignored
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Message message)
    {
        var group = await _classroomRepository.GetAsync(message.Classroom!.Id);

        foreach (var member in group.Members)
        {
            if (_connections.Keys.Contains(member.Id))
            {
                await Clients
                    .User(_connections[member.Id])
                    .ReceiveMessage(_mapper.Map<MessageDto>(message));
            }
            else
            {
                var user = await _userRepository.GetAsync(member.Id);
                user.UnreadMessages.Add(message);
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
