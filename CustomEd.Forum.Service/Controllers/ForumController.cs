using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomEd.Forum.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.Shared.Response;
using CustomEd.Forum.Service.Dto;
using Microsoft.AspNetCore.Authorization;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.JWT;
using Microsoft.AspNetCore.SignalR;
using CustomEd.Forum.Service.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace CustomEd.Forum.Service.Controllers
{
    [Authorize(Policy = "MemberOnly")]
    [ApiController]
    [Route("/api/{classRoomId}/forum")]
    public class ForumController : ControllerBase
    {
        private readonly IGenericRepository<Message> _messageRepository;
        private readonly IGenericRepository<Model.User> _userRepository;
        private readonly IGenericRepository<Classroom> _classroomRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<ForumHub, IForumClient> _forumHub;
        

        public ForumController(IGenericRepository<Message> messageRepository, IGenericRepository<Model.User> userRepository, IGenericRepository<Classroom> classroomRepository, IMapper mapper, IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IHubContext<ForumHub, IForumClient> forumHub)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _forumHub = forumHub;
        }

        
        private static async Task SendMessageToGroup(Message message)
        {
            try
            {
                var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5082/forumHub")
                .Build();
                await connection.StartAsync();
                await connection.InvokeAsync("SendMessage", message);
                await connection.StopAsync();
                await connection.DisposeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
       

        [HttpPost("send-message")]
        public async Task<ActionResult<SharedResponse<MessageDto>>> SendMessage(Guid classRoomId, [FromBody] CreateMessageDto createMessageDto)
        {
            var validationResult = await new CreateMessageDtoValidator(_userRepository, _classroomRepository, _messageRepository).ValidateAsync(createMessageDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(SharedResponse<MessageDto>.Fail(null, errors));
            }
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId(); 

            createMessageDto.SenderId = currentUserId;
            createMessageDto.ClassroomId = classRoomId;

            var message = _mapper.Map<Message>(createMessageDto);
            await _messageRepository.CreateAsync(message);
            await SendMessageToGroup(message);

            
            var messageDto = _mapper.Map<MessageDto>(message);
            return Ok(SharedResponse<MessageDto>.Success(messageDto, "Message sent successfully."));
            
        }

        [HttpPut("update-message")]
        public async Task<ActionResult<SharedResponse<MessageDto>>> UpdateMessage(Guid classRoomId, [FromBody] UpdateMessageDto updateMessageDto)
        {
            var validationResult = await new UpdateMessageDtoValidator(_userRepository, _classroomRepository, _messageRepository).ValidateAsync(updateMessageDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(SharedResponse<MessageDto>.Fail(null, errors));
            }

            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();

            if(updateMessageDto.SenderId != currentUserId)
            {
                return Unauthorized(SharedResponse<MessageDto>.Fail(null, new List<string> { "You are not authorized to update this message." }));
            }

            updateMessageDto.SenderId = currentUserId;
            updateMessageDto.ClassroomId = classRoomId;
            var message = _mapper.Map<Message>(updateMessageDto);
            await _messageRepository.UpdateAsync(message);

            var messageDto = _mapper.Map<MessageDto>(message);
            return Ok(SharedResponse<MessageDto>.Success(messageDto, "Message updated successfully."));
        }

        [HttpDelete("delete-message")]
        public async Task<ActionResult<SharedResponse<MessageDto>>> DeleteMessage(Guid classRoomId, Guid messageId)
        {
            var message = await _messageRepository.GetAsync(messageId);
            if (message == null)
            {
                return NotFound(SharedResponse<MessageDto>.Fail(null, new List<string> { "Message not found." }));
            }

            if(message.Classroom!.Id != classRoomId)
            {
                return BadRequest(SharedResponse<MessageDto>.Fail(null, new List<string> { "Message does not belong to this classroom." }));
            }
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if (message.Sender.Id != currentUserId)
            {
                return Unauthorized(SharedResponse<MessageDto>.Fail(null, new List<string> { "You are not authorized to delete this message." }));
            }

            await _messageRepository.RemoveAsync(message);
            var messageDto = _mapper.Map<MessageDto>(message);
            return Ok(SharedResponse<MessageDto>.Success(messageDto, "Message deleted successfully."));
        }

        [HttpGet("all-messages")]
        public async Task<ActionResult<SharedResponse<List<MessageDto>>>> GetAllMessages(Guid classRoomId)
        {
            var messages = await _messageRepository.GetAllAsync(u => u.Classroom!.Id == classRoomId);
            var messageDtos = _mapper.Map<List<MessageDto>>(messages);
            return Ok(messageDtos);
        }



        [HttpGet("unread-messages")]
        public async Task<ActionResult<SharedResponse<List<Message>>>> GetUnreadMessages(Guid classRoomId)
        {
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return NotFound(SharedResponse<List<Message>>.Fail(null, new List<string> { "User not found." }));
            }
            var messages = user.UnreadMessages;
            user.UnreadMessages = new List<Message>();
            await _userRepository.UpdateAsync(user);
            var messageDtos = _mapper.Map<List<Message>>(messages);
            return Ok(SharedResponse<List<Message>>.Success(messageDtos, "Unread messages retrieved successfully."));

        }

        [HttpGet("unread-messages-count")]
        public async Task<ActionResult<SharedResponse<int>>> GetUnreadMessagesCount(Guid classRoomId)
        {
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return NotFound(SharedResponse<int>.Fail("User not found", new List<string> { "User not found." }));
            }
            var unreadMessagesCount = user.UnreadMessages.Count;
            return Ok(SharedResponse<int>.Success(unreadMessagesCount, "Unread messages count retrieved successfully."));
        }

        [HttpGet("message-replies")]
        public async Task<ActionResult<SharedResponse<List<MessageDto>>>> GetMessageReplies(Guid messageId)
        {
            var message = await _messageRepository.GetAsync(messageId);
            if (message == null)
            {
                return NotFound(SharedResponse<List<MessageDto>>.Fail(null, new List<string> { "Message not found." }));
            }
            var replies = await _messageRepository.GetAllAsync(m => m.ThreadParent == messageId);
            var replyDtos = _mapper.Map<List<MessageDto>>(replies);
            return Ok(SharedResponse<List<MessageDto>>.Success(replyDtos, "Replies retrieved successfully."));
        }
    }
}
