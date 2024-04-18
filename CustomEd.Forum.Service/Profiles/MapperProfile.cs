using AutoMapper;
using CustomEd.Forum.Service.Model;
using CustomEd.Forum.Service.Dto;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.User.Teacher.Events;
using CustomEd.Shared.Model;
using CustomEd.User.Student.Events;

namespace CustomEd.Forum.Service.Profiles
{
    public class MapperProfile : Profile
    {
        private readonly IGenericRepository<Model.User> _userRepository;
        private readonly IGenericRepository<Model.Classroom> _classroomRepository;
        public MapperProfile(IGenericRepository<Model.User> userRepository, IGenericRepository<Model.Classroom> classroomRepository)
        {
            _userRepository = userRepository;
            _classroomRepository = classroomRepository;

            CreateMap<Model.User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.UnreadMessages, opt => opt.MapFrom(src => src.UnreadMessages.Select(m => m.Id).ToList()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
            
            CreateMap<TeacherCreatedEvent, Model.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Teacher));
            
            CreateMap<StudentCreatedEvent, Model.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Student));

            CreateMap<Model.Classroom, ClassroomDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
            

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.ClassroomId, opt => opt.MapFrom(src => src.Classroom!.Id));

            CreateMap<CreateMessageDto, Message>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(async (src, dest, destMember, context) => await _userRepository!.GetAsync(src.SenderId)))
                .ForMember(dest => dest.Classroom, opt => opt.MapFrom(async (src, dest, destMember, context) => await _classroomRepository!.GetAsync(src.ClassroomId)));
            
            CreateMap<UpdateMessageDto, Message>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));
            
            }
    }
}
