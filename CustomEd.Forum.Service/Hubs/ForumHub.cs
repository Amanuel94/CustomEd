using System.Collections.Concurrent;
using System.Threading.Tasks;
using AutoMapper;
using CustomEd.Forum.Service.Dto;
using CustomEd.Forum.Service.Hubs.Interfaces;
using CustomEd.Forum.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using Microsoft.AspNetCore.SignalR;
using MassTransit;
using CustomEd.Contracts.Notification.Events;

namespace CustomEd.Forum.Service.Hubs;
public class ForumHub : Hub<IForumClient>
{
    private readonly ConcurrentDictionary<Guid, string> _connections =
        new ConcurrentDictionary<Guid, string>();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<Model.Classroom> _classroomRepository;
    private readonly IGenericRepository<Teacher> _teacherRepository;
    private readonly IGenericRepository<Student> _studentRepository;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private readonly IGenericRepository<Message> _messageRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ForumHub(
        ConcurrentDictionary<Guid, string> connections,
        IHttpContextAccessor httpContextAccessor,
        IGenericRepository<Model.Classroom> classroomRepository,
        IGenericRepository<Teacher> teacherRepository,
        IGenericRepository<Student> studentRepository,
        IGenericRepository<Message> messageRepository,
        IMapper mapper,
        IJwtService jwtService,
        IPublishEndpoint publishEndpoint
    )
    {
        _connections = connections;
        _httpContextAccessor = httpContextAccessor;
        _classroomRepository = classroomRepository;
        _mapper = mapper;
        _jwtService = jwtService;
        _teacherRepository = teacherRepository;
        _studentRepository = studentRepository;
        _messageRepository = messageRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"ConnectionId {Context.ConnectionId} is connecting. . .");
        foreach (var item in await _studentRepository.GetAllAsync())
        {
         Console.WriteLine(item.Id);   
        }
        foreach (var item in await _classroomRepository.GetAllAsync())
        {
            foreach (var st in item.Members)
            {
             Console.WriteLine(st.Id);      
            }
         
        }
        Console.WriteLine("-----------------------");
        try
        {
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
            Console.WriteLine($"line 58: {currentUserId}");
            Console.WriteLine(userRole);
            if (userRole == Role.Teacher)
            {
                var user = await _teacherRepository.GetAsync(currentUserId);
                if (user == null)
                {
                    // return;
                    throw new Exception("User not found");
                }

                _connections.TryAdd(currentUserId, Context.ConnectionId);

                var groups = await _classroomRepository.GetAllAsync(x =>
                    x.Members.Select(m => m.Id).Contains(currentUserId)
                    || x.Creator.Id == currentUserId
                );

                foreach (var group in groups)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
                }

            //     if (user.UnreadMessages == null)
            //     {
            //         user.UnreadMessages = new List<Message>();
            //     }
            //     foreach (var message in user.UnreadMessages)
            //     {
            //         await Clients.Caller.ReceiveMessage(_mapper.Map<MessageDto>(message));
            //     }
            //     user.UnreadMessages.Clear();
            //     await _teacherRepository.UpdateAsync(user);
            }
            else
            {
                var user = await _studentRepository.GetAsync(currentUserId);
                if (user == null)
                {
                    // return;
                    throw new Exception("User not found");
                }

                
                _connections.TryAdd(currentUserId, Context.ConnectionId);

                var groups = await _classroomRepository.GetAllAsync(x =>
                    x.Members.Select(m => m.Id).Contains(currentUserId)
                    || x.Creator.Id == currentUserId
                );

                foreach (var group in groups)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
                }

                // if (user.UnreadMessages == null)
                // {
                //     user.UnreadMessages = new List<Message>();
                // }
                // foreach (var message in user.UnreadMessages)
                // {
                //     Console.WriteLine(user.Id);
                //     Console.WriteLine(message.Content);
                //     // await Clients.Caller.ReceiveMessage(_mapper.Map<MessageDto>(message));
                //     await Clients.Client(_connections[user.Id]).ReceiveMessage(_mapper.Map<MessageDto>(message));
                // }
                // user.UnreadMessages.Clear();
                // await _studentRepository.UpdateAsync(user);
            }
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
            Console.WriteLine($"{Context.ConnectionId} is disconnected. . .");
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            _connections.TryRemove(userId, out _);
            var groups = await _classroomRepository.GetAllAsync(x =>
                x.Members.Select(m => m.Id).Contains(userId) || x.Creator.Id == userId
            );
            foreach (var group in groups)
            {
                Console.WriteLine($"Removing from group: {group.Id}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Id.ToString());
            }
        }
        catch (System.Exception e)
        {
            // ignored
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto messageDto)
    {
        // if(messageDto != null)
        // {
        //     Console.WriteLine("Message is not null");
        //     Console.WriteLine(messageDto.SenderRole);
        //     return;
        // }
        var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
        // Console.WriteLine("Role: " + userRole);
        // var message = _mapper.Map<Message>(messageDto);
        
        Message message;
        if (messageDto.SenderRole == Role.Student)
        {
            var validator = new CreateMessageDtoValidator<Student>(_studentRepository, _classroomRepository, _messageRepository); 
            Console.WriteLine(messageDto.SenderId);
            Console.WriteLine(messageDto.ClassroomId);
            Console.WriteLine(messageDto.Content);       
            var result = await validator.ValidateAsync(messageDto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return;
            }
            message = new Message
            {
                Content = messageDto.Content,
                Sender = await _studentRepository.GetAsync(
                    new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId()
                ),
                Classroom = await _classroomRepository.GetAsync(messageDto.ClassroomId),
                ThreadParent = messageDto.ThreadParent
            };
        }
        else
        {
            var validator = new CreateMessageDtoValidator<Teacher>(_teacherRepository, _classroomRepository, _messageRepository);        
            var result = await validator.ValidateAsync(messageDto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return;
            }
            message = new Message
            {
                Content = messageDto.Content,
                Sender = await _teacherRepository.GetAsync(
                    new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId()
                ),
                Classroom = await _classroomRepository.GetAsync(messageDto.ClassroomId),
                ThreadParent = messageDto.ThreadParent
            };
        }
        Console.WriteLine("Message: " + message.Content);
        var group = await _classroomRepository.GetAsync(message.Classroom!.Id);
        Console.WriteLine("Group: " + group.Name);
        // foreach (var member in group.Members)
        // {
        //     Console.WriteLine("Member: " + member.Id);
        //     if (_connections.Keys.Contains(member.Id))
        //     {
        //         Console.WriteLine("Member is connected");
        //         await Clients
        //             .User(_connections[member.Id])
        //             .ReceiveMessage(_mapper.Map<MessageDto>(message));
        //     }
        //     else
        //     {
        //         var user = await _studentRepository.GetAsync(member.Id);
        //         user.UnreadMessages.Add(message);
        //         await _studentRepository.UpdateAsync(user);
        //     }
        //     await _messageRepository.CreateAsync(message);
        // }
        // var creator = group.Creator.Id;
        // await Clients.User(_connections[creator]).ReceiveMessage(_mapper.Map<MessageDto>(message));
        // await _messageRepository.CreateAsync(message);
        await Clients
            .Group(messageDto.ClassroomId.ToString())
            .ReceiveMessage(_mapper.Map<MessageDto>(message));
        foreach (var member in group.Members)
        {
            if (!_connections.Keys.Contains(member.Id))
            {
                Console.WriteLine("Saving message to member");
                Console.WriteLine(member.Id);
                Console.WriteLine(message.Content);
                var user = await _studentRepository.GetAsync(member.Id);
                if (user.UnreadMessages == null)
                {
                    user.UnreadMessages = new List<Message>();
                }
                user.UnreadMessages.Add(message);
                await _studentRepository.UpdateAsync(user);
            }
        }
        if (!_connections.Keys.Contains(group.Creator.Id))
        {
            var user = await _teacherRepository.GetAsync(group.Creator.Id);
            Console.WriteLine("Saving message to creator");
            Console.WriteLine(user.Id);
            Console.WriteLine(message.Content);
            if (user.UnreadMessages == null)
            {
                user.UnreadMessages = new List<Message>();
            }
            user.UnreadMessages.Add(message);
            await _teacherRepository.UpdateAsync(user);
        }

        if(message.ThreadParent != null && message.ThreadParent != Guid.Empty)
        {
            var parentMessage = await _messageRepository.GetAsync(message.ThreadParent);
            if (!_connections.Keys.Contains(parentMessage.Sender.Id))
            {
                var user = await _studentRepository.GetAsync(parentMessage.Sender.Id);
                if (user.UnreadMessages == null)
                {
                    user.UnreadMessages = new List<Message>();
                }
                user.UnreadMessages.Add(message);
                await _studentRepository.UpdateAsync(user);
                var notifyUserEvent = new NotifyUserEvent
                    {
                        ReceiverId = user.Id,
                        Description = "You have a new message",
                        Type = $"{message.Content.Substring(0, 30)}",
                        role = user.Role
                    };
                await _publishEndpoint.Publish(notifyUserEvent);
            }
        }
    }
}
