using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomEd.Forum.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using AutoMapper;
using CustomEd.Shared.Response;
using CustomEd.Forum.Service.Dto;

namespace CustomEd.Forum.Service.Controllers
{
    [ApiController]
    [Route("/api/{classRoomId}/forum")]
    public class ForumController : ControllerBase
    {
        private readonly IGenericRepository<Message> _messageRepository;
        private readonly IGenericRepository<Model.User> _userRepository;
        private readonly IGenericRepository<Classroom> _classroomRepository;
        private readonly IMapper _mapper;

        public ForumController(IGenericRepository<Message> messageRepository, IGenericRepository<Model.User> userRepository, IGenericRepository<Classroom> classroomRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;
        }

        [HttpPost("send-message")]
        public async Task<ActionResult<SharedResponse<MessageDto>>> SendMessage(Guid classRoomId, [FromBody] CreateMessageDto createMessageDto)
        {
            var validationResult = await new CreateMessageDtoValidator(_userRepository, _classroomRepository).ValidateAsync(createMessageDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(SharedResponse<MessageDto>.Fail(null, errors));
            }
            createMessageDto.ClassroomId = classRoomId;
            var message = _mapper.Map<Message>(createMessageDto);
            await _messageRepository.CreateAsync(message);

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



        [HttpGet("unread-messages/{userId}")]
        public async Task<ActionResult<SharedResponse<List<Message>>>> GetUnreadMessages(Guid classRoomId, Guid userId)
        {
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

        [HttpGet("unread-messages-count/{userId}")]
        public async Task<ActionResult<SharedResponse<int>>> GetUnreadMessagesCount(Guid classRoomId, Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                return NotFound(SharedResponse<int>.Fail("User not found", new List<string> { "User not found." }));
            }
            var unreadMessagesCount = user.UnreadMessages.Count;
            return Ok(SharedResponse<int>.Success(unreadMessagesCount, "Unread messages count retrieved successfully."));
        }
    }
}
