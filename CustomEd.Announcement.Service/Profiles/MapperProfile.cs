using AutoMapper;
using CustomEd.Announcement.Service.DTOs;
using CustomEd.Announcement.Service.Model;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Profiles
{
    public class MapperProfile : Profile
    {
        private readonly IGenericRepository<ClassRoom> _classroomRepository;
        public MapperProfile(IGenericRepository<ClassRoom> classroomRepository)
        {
            _classroomRepository = classroomRepository;

            CreateMap<Model.Announcement, AnnouncementDto>().ReverseMap();
            CreateMap<CreateAnnouncementDto, Model.Announcement>()
                .ForMember(dest => dest.ClassRoom, opt => opt.MapFrom(src => _classroomRepository.GetAsync(src.ClassRoomId)));
            CreateMap<UpdateAnnouncementDto, Model.Announcement>()
                .ForMember(dest => dest.ClassRoom, opt => opt.MapFrom(src => _classroomRepository.GetAsync(src.ClassRoomId)));

            CreateMap<ClassRoom, ClassroomCreatedEvent>().ReverseMap();
            CreateMap<ClassRoom, ClassroomUpdatedEvent>().ReverseMap();

        }
    }
}