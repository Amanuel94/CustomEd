using AutoMapper;
using CustomEd.RTNotification.Service.Dto;
using CustomEd.RTNotification.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CustomEd.RTNotification.Service.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly IGenericRepository<Notification> _notificationRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        private readonly IGenericRepository<Model.User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public NotificationController(IGenericRepository<Notification> notificationRepository, IGenericRepository<Classroom> classroomRepository, IGenericRepository<Model.User> userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _notificationRepository = notificationRepository;
            _classroomRepository = classroomRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
        }

        [HttpGet("unread")]
        public async Task<ActionResult<SharedResponse<IEnumerable<NotificationDto>>>> GetUnreadNotifications(Guid classRoomId)
        {
            var currentUserId = new IdentityProvider(_httpContextAccessor, _jwtService).GetUserId();
            var user = await _userRepository.GetAsync(currentUserId);
            if (user == null)
            {
                return NotFound(SharedResponse<IEnumerable<NotificationDto>>.Fail("User not found", null));
            }
            var notifications = _mapper.Map<IEnumerable<NotificationDto>>(user.UnreadNotifications);
            if(user.UnreadNotifications != null)
            {
                user.UnreadNotifications.Clear();
                await _userRepository.UpdateAsync(user);
            }
            return Ok(SharedResponse<IEnumerable<NotificationDto>>.Success(notifications, null));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedResponse<NotificationDto>>> GetNotification(Guid classRoomId, Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification == null)
            {
                return NotFound(SharedResponse<NotificationDto>.Fail("Notification not found", null));
            }
            return Ok(SharedResponse<NotificationDto>.Success(_mapper.Map<NotificationDto>(notification), null));
        }
    }
}