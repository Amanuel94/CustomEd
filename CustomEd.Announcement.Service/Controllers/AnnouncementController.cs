using AutoMapper;
using CustomEd.Announcement.Service.DTOs;
using CustomEd.Announcement.Service.DTOs.Validtion;
using CustomEd.Announcement.Service.Model;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Mail;
using MassTransit;
using CustomEd.Contracts.Notification.Events;

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
        private readonly IPublishEndpoint _publishEndpoint;
        public AnnouncementController(IGenericRepository<Model.Announcement> announcementRepository, IGenericRepository<Model.ClassRoom> classRoomRepository, IGenericRepository<Model.Teacher> teacherRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _announcementRepository = announcementRepository;
            _classRoomRepository = classRoomRepository;
            _teacherRepository = teacherRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            
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

            var notifyClassroomEvent = new NotifyClassroomEvent{
                    ClassroomId = Guid.Parse(classRoomId),
                    Description = "An Announcement has been posted",
                    Type = "Announcement"
            };
            await _publishEndpoint.Publish(notifyClassroomEvent);
            var announcementDto = _mapper.Map<AnnouncementDto>(announcement);
            return Ok(SharedResponse<AnnouncementDto>.Success(announcementDto, "Announcement Created"));
            
        }

        [HttpPost("{classRoomId}/announcements/attach/{id}")]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<AnnouncementDto>>> Attach(string classRoomId, Guid id, [FromForm] List<IFormFile> attachements)
        {
            var announcement = await _announcementRepository.GetAsync(id);
            if (announcement == null)
            {
                return NotFound(SharedResponse<AnnouncementDto>.Fail("Announcement not found", null));
            }
            if (announcement.ClassRoom.Id.ToString() != classRoomId)
            {
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Cannot attach announcement in a different classroom", null));
            }
            string directoryPath = Path.Combine("Uploads", announcement.Id.ToString());
            
            var totalMegaBytes = attachements.Sum(x => x.Length) / 1024 / 1024;
            if (totalMegaBytes > 10)
            {
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Total size of attachments must not exceed 10MB", null));
            }
            Console.WriteLine("Directory Path: " + directoryPath);
            Console.WriteLine(attachements.Count);
            if (attachements != null)
            {
                foreach (var file in attachements)
                {
                    var tempName = Guid.NewGuid().ToString();
                    string filePath = Path.Combine(directoryPath,
                                                   tempName + Path.GetExtension(file.FileName));
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".pdf")
                        {
                            Console.WriteLine("Writing...");
                            await file.CopyToAsync(stream);
                        }
                        else{
                            return BadRequest(SharedResponse<AnnouncementDto>.Fail("Only PDF files are allowed", null));
                        }
                    }

                    if(announcement.Attachements == null)
                    {
                        announcement.Attachements = new List<string>();
                    }

                    announcement.Attachements.Add(tempName);
                    Console.WriteLine(announcement.Attachements.Count);
                    Console.WriteLine("File Path: " + filePath);
                }
            }
            var notifyClassroomEvent = new NotifyClassroomEvent{
                    ClassroomId = Guid.Parse(classRoomId),
                    Description = $"An Attachment has been added to an Announcement in classroom {announcement.ClassRoom.Name}",
                    Type = "Announcement"
            };
            await _announcementRepository.UpdateAsync(announcement);
            return NoContent();
        }

        [HttpDelete("{classRoomId}/announcements/{id}/attachments/{fileName}")]
        [Authorize(policy:"CreatorOnlyPolicy")]
        public async Task<ActionResult<SharedResponse<AnnouncementDto>>> RemoveAttachment(string classRoomId, Guid id, string fileName)
        {
            var announcement = await _announcementRepository.GetAsync(id);
            if (announcement == null)
            {
                return NotFound(SharedResponse<AnnouncementDto>.Fail("Announcement not found", null));
            }
            if (announcement.ClassRoom.Id.ToString() != classRoomId)
            {
                return BadRequest(SharedResponse<AnnouncementDto>.Fail("Cannot remove attachment in a different classroom", null));

            }
            string directoryPath = Path.Combine("Uploads", announcement.Id.ToString());
            string filePath = Path.Combine(directoryPath, fileName + ".pdf");
            if (!Directory.Exists(directoryPath) || !System.IO.File.Exists(filePath))
            {
                return NotFound(SharedResponse<AnnouncementDto>.Fail("Attachment not found", null));
            }
            System.IO.File.Delete(filePath);
            announcement.Attachements!.Remove(fileName);
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