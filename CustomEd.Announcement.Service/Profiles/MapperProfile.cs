using AutoMapper;
using CustomEd.Announcement.Service.DTOs;
using CustomEd.Announcement.Service.Model;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;

namespace CustomEd.Announcement.Service.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Model.Announcement, AnnouncementDto>()
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachements));
            CreateMap<CreateAnnouncementDto, Model.Announcement>();
            CreateMap<UpdateAnnouncementDto, Model.Announcement>();
            CreateMap<ClassRoom, ClassroomCreatedEvent>().ReverseMap();
            CreateMap<ClassRoom, ClassroomUpdatedEvent>().ReverseMap();

        }
    }
}