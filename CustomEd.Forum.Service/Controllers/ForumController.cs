using System.Collections.Generic;
using Amazon.Runtime;
using AutoMapper;
using CustomEd.Forum.Service.Dto;
using CustomEd.Forum.Service.Hubs.Interfaces;
using CustomEd.Forum.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Model;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using CustomEd.Forum.Service.Hubs;

namespace CustomEd.Forum.Service.Controllers
{
    [Authorize(Policy = "MemberOnly")]
    [ApiController]
    [Route("/api/{classRoomId}/forum")]
    public class ForumController : ControllerBase
    {
        private readonly IGenericRepository<Message> _messageRepository;
        private readonly IGenericRepository<Model.Student> _studentRepository;
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<ForumHub, IForumClient> _forumHub;

        public ForumController(
            IGenericRepository<Message> messageRepository,
            IGenericRepository<Student> studentRepository,
            IGenericRepository<Teacher> teacherRepository,
            IGenericRepository<Model.Classroom> classroomRepository,
            IMapper mapper,
            IJwtService jwtService,
            IHttpContextAccessor httpContextAccessor,
            IHubContext<ForumHub, IForumClient> forumHub
        )
        {
            _messageRepository = messageRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _classroomRepository = classroomRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _forumHub = forumHub;
        }

        private static async Task SendMessageToGroup(CreateMessageDto message)
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

        [HttpPost("create-message")]
        public async Task<ActionResult<SharedResponse<MessageDto>>> SendMessage(
            Guid classRoomId,
            [FromBody] CreateMessageDto createMessageDto
        )
        {
            if (createMessageDto.SenderRole == Role.Student)
            {
                var validationResult = await new CreateMessageDtoValidator<Student>(
                    _studentRepository,
                    _classroomRepository,
                    _messageRepository
                ).ValidateAsync(createMessageDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(SharedResponse<MessageDto>.Fail(null, errors));
                }
                var currentUserId = new IdentityProvider(
                    _httpContextAccessor,
                    _jwtService
                ).GetUserId();

                createMessageDto.SenderId = currentUserId;
                createMessageDto.ClassroomId = classRoomId;

                var message = _mapper.Map<Message>(createMessageDto);
                message.Sender = await _studentRepository.GetAsync(currentUserId);
                if(message.Sender == null)
                {
                    return BadRequest(
                        SharedResponse<MessageDto>.Fail(
                            null,
                            new List<string> { "Student not found." }
                        )
                    );
                }
                message.Classroom = await _classroomRepository.GetAsync(classRoomId);

                await _messageRepository.CreateAsync(message);
                await SendMessageToGroup(createMessageDto);

                var messageDto = _mapper.Map<MessageDto>(message);
                return Ok(
                    SharedResponse<MessageDto>.Success(messageDto, "Message sent successfully.")
                );
            }
            else if (createMessageDto.SenderRole == Role.Teacher)
            {
                var validationResult = await new CreateMessageDtoValidator<Teacher>(
                    _teacherRepository,
                    _classroomRepository,
                    _messageRepository
                ).ValidateAsync(createMessageDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(SharedResponse<MessageDto>.Fail(null, errors));
                }
                var currentUserId = new IdentityProvider(
                    _httpContextAccessor,
                    _jwtService
                ).GetUserId();

                createMessageDto.SenderId = currentUserId;
                createMessageDto.ClassroomId = classRoomId;


                var message = _mapper.Map<Message>(createMessageDto);
                message.Sender = await _teacherRepository.GetAsync(currentUserId);
                message.Classroom = await _classroomRepository.GetAsync(classRoomId);
                if(message.Sender == null)
                {
                    return BadRequest(
                        SharedResponse<MessageDto>.Fail(
                            null,
                            new List<string> { "Teacher not found." }
                        )
                    );
                }

                await _messageRepository.CreateAsync(message);
                await SendMessageToGroup(createMessageDto);

                var messageDto = _mapper.Map<MessageDto>(message);
                return Ok(
                    SharedResponse<MessageDto>.Success(messageDto, "Message sent successfully.")
                );
            }
            else if (createMessageDto.SenderRole == Role.Admin)
            {
                return BadRequest(
                    SharedResponse<MessageDto>.Fail(
                        null,
                        new List<string> { "Admin cannot send messages." }
                    )
                );
            }
            else
            {
                return BadRequest(
                    SharedResponse<MessageDto>.Fail(
                        null,
                        new List<string> { "Invalid sender role." }
                    )
                );
            }
        }

        [HttpPut("update-message")]
        public async Task<ActionResult<SharedResponse<MessageDto>>> UpdateMessage(
            Guid classRoomId,
            [FromBody] UpdateMessageDto updateMessageDto
        )
        {
            if (updateMessageDto.SenderRole == Role.Student)
            {
                var validationResult = await new UpdateMessageDtoValidator<Student>(
                    _studentRepository,
                    _classroomRepository,
                    _messageRepository
                ).ValidateAsync(updateMessageDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(SharedResponse<MessageDto>.Fail(null, errors));
                }
                var currentUserId = new IdentityProvider(
                    _httpContextAccessor,
                    _jwtService
                ).GetUserId();

                updateMessageDto.SenderId = currentUserId;
                updateMessageDto.ClassroomId = classRoomId;

                var message = _mapper.Map<Message>(updateMessageDto);
                message.Sender = await _studentRepository.GetAsync(currentUserId);
                await _messageRepository.UpdateAsync(message);
                // await SendMessageToGroup(message);

                var messageDto = _mapper.Map<MessageDto>(message);
                return Ok(
                    SharedResponse<MessageDto>.Success(messageDto, "Message updated successfully.")
                );
            }
            else if (updateMessageDto.SenderRole == Role.Teacher)
            {
                var validationResult = await new UpdateMessageDtoValidator<Teacher>(
                    _teacherRepository,
                    _classroomRepository,
                    _messageRepository
                ).ValidateAsync(updateMessageDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(SharedResponse<MessageDto>.Fail(null, errors));
                }
                var currentUserId = new IdentityProvider(
                    _httpContextAccessor,
                    _jwtService
                ).GetUserId();

                updateMessageDto.SenderId = currentUserId;
                updateMessageDto.ClassroomId = classRoomId;

                var message = _mapper.Map<Message>(updateMessageDto);
                message.Sender = await _teacherRepository.GetAsync(currentUserId);
                await _messageRepository.UpdateAsync(message);
                // await SendMessageToGroup(message);

                var messageDto = _mapper.Map<MessageDto>(message);
                return Ok(
                    SharedResponse<MessageDto>.Success(messageDto, "Message updated successfully.")
                );
            }

            else if (updateMessageDto.SenderRole == Role.Admin)
            {
                return BadRequest(
                    SharedResponse<MessageDto>.Fail(
                        null,
                        new List<string> { "Admin cannot send messages." }
                    )
                );
            }
            else
            {
                return BadRequest(
                    SharedResponse<MessageDto>.Fail(
                        null,
                        new List<string> { "Invalid sender role." }
                    )
                );
            }
           
        }

        [HttpDelete("delete-message")]
        public async Task<ActionResult<SharedResponse<MessageDto>>> DeleteMessage(
            Guid classRoomId,
            Guid messageId
        )
        {
            var message = await _messageRepository.GetAsync(messageId);
            if (message == null)
            {
                return NotFound(
                    SharedResponse<MessageDto>.Fail(null, new List<string> { "Message not found." })
                );
            }

            if (message.Classroom!.Id != classRoomId)
            {
                return BadRequest(
                    SharedResponse<MessageDto>.Fail(
                        null,
                        new List<string> { "Message does not belong to this classroom." }
                    )
                );
            }
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            if (message.Sender.Id != currentUserId)
            {
                return Unauthorized(
                    SharedResponse<MessageDto>.Fail(
                        null,
                        new List<string> { "You are not authorized to delete this message." }
                    )
                );
            }

            await _messageRepository.RemoveAsync(message);
            var messageDto = _mapper.Map<MessageDto>(message);
            return Ok(
                SharedResponse<MessageDto>.Success(messageDto, "Message deleted successfully.")
            );
        }

        [HttpGet("all-messages")]
        public async Task<ActionResult<PaginatedResponse<MessageDto>>> GetAllMessages(
            Guid classRoomId,
            int page = 1,
            int pageSize = 10
        )
        {
            var messages = await _messageRepository.GetAllAsync(u =>
            u.Classroom!.Id == classRoomId
            );
            
            var totalItems = messages.ToList().Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var pagedMessages = messages
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
            var messageDtos = _mapper.Map<List<MessageDto>>(pagedMessages);
            var pagedResponse = new CusotmEd.Forum.Responses.PaginatedResponse<MessageDto>{  
                Total = totalItems,
                Page = page,
                PageSize = pageSize,
                Data = messageDtos
            };
            return Ok(pagedResponse);
            }
            
            
        

        [HttpGet("unread-messages")]
        public async Task<ActionResult<SharedResponse<List<Message>>>> GetUnreadMessages(
            Guid classRoomId
        )
        {
            
        
            var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var userRole = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserRole();
            if(userRole == Role.Student)
            {
                var user = await _studentRepository.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(
                        SharedResponse<List<Message>>.Fail(null, new List<string> { "User not found." })
                    );
                }
                var messages = user.UnreadMessages;
                user.UnreadMessages = new List<Message>();
                await _studentRepository.UpdateAsync(user);
                var messageDtos = _mapper.Map<List<Message>>(messages);
                return Ok(
                    SharedResponse<List<Message>>.Success(
                        messageDtos,
                        "Unread messages retrieved successfully."
                    )
                );
            }
            else if(userRole == Role.Teacher)
            {
                var user = await _teacherRepository.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(
                        SharedResponse<List<Message>>.Fail(null, new List<string> { "User not found." })
                    );
                }
                var messages = user.UnreadMessages;
                user.UnreadMessages = new List<Message>();
                await _teacherRepository.UpdateAsync(user);
                var messageDtos = _mapper.Map<List<Message>>(messages);
                return Ok(
                    SharedResponse<List<Message>>.Success(
                        messageDtos,
                        "Unread messages retrieved successfully."
                    )
                );
            }
            else if(userRole == Role.Admin)
            {
                // var user = await _userRepository.GetAsync(userId);
                // if (user == null)
                // {
                //     return NotFound(
                //         SharedResponse<List<Message>>.Fail(null, new List<string> { "User not found." })
                //     );
                // }
                // var messages = user.UnreadMessages;
                // user.UnreadMessages = new List<Message>();
                // await _userRepository.UpdateAsync(user);
                // var messageDtos = _mapper.Map<List<Message>>(messages);
                // return Ok(
                //     SharedResponse<List<Message>>.Success(
                //         messageDtos,
                //         "Unread messages retrieved successfully."
                //     )
                // );
                 return BadRequest(
                    SharedResponse<List<Message>>.Fail(null, new List<string> { "No Admin Messages yet." })
                );
            }
            else
            {
                return BadRequest(
                    SharedResponse<List<Message>>.Fail(null, new List<string> { "Invalid user role." })
                );
            }

        }

        // [HttpGet("unread-messages-count")]
        // public async Task<ActionResult<SharedResponse<int>>> GetUnreadMessagesCount(
        //     Guid classRoomId
        // )
        // {
        //     var userId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
        //     var user = await _studentRepository.GetAsync(userId);
        //     if (user == null)
        //     {
        //         return NotFound(
        //             SharedResponse<int>.Fail(
        //                 "User not found",
        //                 new List<string> { "User not found." }
        //             )
        //         );
        //     }
        //     var unreadMessagesCount = user.UnreadMessages.Count;
        //     return Ok(
        //         SharedResponse<int>.Success(
        //             unreadMessagesCount,
        //             "Unread messages count retrieved successfully."
        //         )
        //     );
        // }

        [HttpGet("message-replies")]
        public async Task<ActionResult<SharedResponse<List<MessageDto>>>> GetMessageReplies(
            Guid messageId
        )
        {
            var message = await _messageRepository.GetAsync(messageId);
            if (message == null)
            {
                return NotFound(
                    SharedResponse<List<MessageDto>>.Fail(
                        null,
                        new List<string> { "Message not found." }
                    )
                );
            }
            var replies = await _messageRepository.GetAllAsync(m => m.ThreadParent == messageId);
            var replyDtos = _mapper.Map<List<MessageDto>>(replies);
            return Ok(
                SharedResponse<List<MessageDto>>.Success(
                    replyDtos,
                    "Replies retrieved successfully."
                )
            );
        }
    }
}
