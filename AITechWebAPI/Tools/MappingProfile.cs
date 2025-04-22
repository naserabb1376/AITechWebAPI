using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechWebAPI.ViewModels;
using AutoMapper;



namespace AITechWebAPI.Tools
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap(typeof(ListResultObject<>), typeof(ListResultObject<>));
            CreateMap(typeof(RowResultObject<>), typeof(RowResultObject<>));
            CreateMap(typeof(BitResultObject), typeof(BitResultObject));
            CreateMap(typeof(UserRowCustomResponse<>), typeof(UserRowCustomResponse<>));
            CreateMap(typeof(UserListCustomResponse<>), typeof(UserListCustomResponse<>));
            CreateMap(typeof(EventRowCustomResponse<>), typeof(EventRowCustomResponse<>));
            CreateMap(typeof(EventListCustomResponse<>), typeof(EventListCustomResponse<>));
            CreateMap(typeof(NewsRowCustomResponse<>), typeof(NewsRowCustomResponse<>));
            CreateMap(typeof(NewsListCustomResponse<>), typeof(NewsListCustomResponse<>));
            CreateMap(typeof(SettingRowCustomResponse<>), typeof(SettingRowCustomResponse<>));
            CreateMap(typeof(SettingListCustomResponse<>), typeof(SettingListCustomResponse<>));
            CreateMap(typeof(CourseRowCustomResponse<>), typeof(CourseRowCustomResponse<>));
            CreateMap(typeof(CourseListCustomResponse<>), typeof(CourseListCustomResponse<>));
            CreateMap(typeof(TicketRowCustomResponse<>), typeof(TicketRowCustomResponse<>));
            CreateMap(typeof(TicketListCustomResponse<>), typeof(TicketListCustomResponse<>));

            CreateMap<Group, TeacherGroupVM>()
.ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
.ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));
        }
    }

}
