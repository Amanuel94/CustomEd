using AutoMapper;
using CustomEd.Announcement.Service.DTOs;
using CustomEd.Announcement.Service.DTOs.Validtion;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomEd.Announcement.Service.Controllers
{
    [ApiController]
    [Route("/api/classroom")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IGenericRepository<Model.Announcement> _announcementRepository;
        private readonly IGenericRepository<Model.ClassRoom> _classRoomRepository;
        private readonly IGenericRepository<Model.Teacher> _teacherRepository;
        private readonly IMapper _mapper; 
        public AnnouncementController(IGenericRepository<Model.Announcement> announcementRepository, IGenericRepository<Model.ClassRoom> classRoomRepository, IGenericRepository<Model.Teacher> teacherRepository, IMapper mapper)
        {
            _announcementRepository = announcementRepository;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
            _mapper = mapper;
            
        }

        
        [HttpGet("{classRoomId}/announcements")]
        [Authorize(policy:"MemberOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<List<AnnouncementDto>>>> GetAll(string classRoomId)
        {
            Guid classId = Guid.Parse(classRoomId);
            var posts =  await  _announcementRepository.GetAllAsync(x => x.ClassRoom.Id == classId);
            var dtos = _mapper.Map<List<AnnouncementDto>>(posts);
            return SharedResponse<List<AnnouncementDto>>.Success(dtos, "Announcements Retrived");
        }

        [HttpGet("{classRoomId}/announcements/{id}")]
        [Authorize(policy:"MemberOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<SharedResponse<AnnouncementDto>>>>Get(string classRoomId, Guid id)
        {
            Guid classId = Guid.Parse(classRoomId);
            var post = await _announcementRepository.GetAsync(id);
            if (post == null)
            {
                return NotFound(SharedResponse<AnnouncementDto>.Fail("Announcement not found", null));
            }

            var dto = _mapper.Map<AnnouncementDto>(post);
            return Ok(SharedResponse<AnnouncementDto>.Success(dto, "Announcement Retrived"));

        }

        [HttpPost("{classRoomId}/announcements")]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<AnnouncementDto>>> Create(string classRoomId, CreateAnnouncementDto dto)
        {
            var createAnnouncementDtoValidator = new CreateAnnouncementDtoValidator(_classRoomRepository);
            var result = await createAnnouncementDtoValidator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
                BadRequest(SharedResponse<AnnouncementDto>.Fail("Validation Error", errors));

            }
            if(dto.ClassRoomId.ToString() != classRoomId)
            {
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Cannot create announcement in a different classroom", null));
            }
            var announcement = _mapper.Map<Model.Announcement>(dto);
            announcement.ClassRoom = await _classRoomRepository.GetAsync(Guid.Parse(classRoomId));
            await _announcementRepository.CreateAsync(announcement);
            var announcementDto = _mapper.Map<AnnouncementDto>(announcement);
            return Ok(SharedResponse<AnnouncementDto>.Success(announcementDto, "Announcement Created"));
            
        }

        [HttpPut("{classRoomId}/announcements")]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<AnnouncementDto>>> Update(string classRoomId, UpdateAnnouncementDto dto)
        {
            
            var updateAnnouncementDtoValidator = new UpdateAnnouncementDtoValidator(_classRoomRepository, _announcementRepository);
            var result = await updateAnnouncementDtoValidator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Validation Error", errors));

            }

            if(dto.ClassRoomId != Guid.Parse(classRoomId))
            {
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Cannot update announcement in a different classroom", null));
            }
            var announcement = _mapper.Map<Model.Announcement>(dto);
            announcement.ClassRoom = await _classRoomRepository.GetAsync(Guid.Parse(classRoomId));
            await _announcementRepository.UpdateAsync(announcement);
            return NoContent();
        }
        
        [HttpDelete("{classRoomId}/announcements/{id}")]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<IActionResult> Delete(string classRoomId, Guid id)
        {
            var announcement =  await _announcementRepository.GetAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            if (Guid.Parse(classRoomId) != announcement.ClassRoom.Id)
            {
               
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Cannot remove announcement in a different classroom", null));
        
            }
            await _announcementRepository.RemoveAsync(announcement);
            return NoContent();

            
        }
    }
}