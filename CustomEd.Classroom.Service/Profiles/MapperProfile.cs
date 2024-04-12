using AutoMapper;
using CustomEd.Classroom.Service.DTOs;
using CustomEd.Classroom.Service.Model;
using CustomEd.Contracts.Classroom.Events;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Contracts.Teacher.Events;
using CustomEd.User.Student.Events;
using CustomEd.User.Teacher.Events;

public class MapperProfile : Profile
{
    private readonly IGenericRepository<Teacher> _teacherRepository;
    public MapperProfile(IGenericRepository<Teacher> teacherRepository)
    {
        _teacherRepository = teacherRepository;

        CreateMap<Classroom, ClassroomDto>().ReverseMap();
        CreateMap<CreateClassroomDto, Classroom>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(async (src, dest, destMember, context) => await _teacherRepository.GetAsync(src.CreatorId)));

        CreateMap<UpdateClassroomDto, Classroom>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(async (src, dest, destMember, context) => await _teacherRepository.GetAsync(src.CreatorId)));

        CreateMap<TeacherCreatedEvent, Teacher>().ReverseMap();
        CreateMap<TeacherUpdatedEvent, Teacher>().ReverseMap();

        CreateMap<StudentCreatedEvent, Student>().ReverseMap();
        CreateMap<StudentUpdatedEvent, Student>().ReverseMap();

        CreateMap<ClassroomCreatedEvent, Classroom>().ReverseMap();
        CreateMap<ClassroomUpdatedEvent, Classroom>().ReverseMap();
        
        
    }
}