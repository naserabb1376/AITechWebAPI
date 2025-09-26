using AiTech.Domains;
using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.JSInterop;



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
            CreateMap(typeof(ArticleRowCustomResponse<>), typeof(ArticleRowCustomResponse<>));
            CreateMap(typeof(ArticleListCustomResponse<>), typeof(ArticleListCustomResponse<>));
            CreateMap(typeof(TicketRowCustomResponse<>), typeof(TicketRowCustomResponse<>));
            CreateMap(typeof(TicketListCustomResponse<>), typeof(TicketListCustomResponse<>));

            CreateMap<Group, GroupVM>()
.ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<Address, AddressVM>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName));

            CreateMap<AdminReport, AdminReportVM>()
               .ForMember(dest => dest.AdminUserName, opt => opt.MapFrom(src => $"{src.Admin.FirstName} {src.Admin.LastName}"));

            CreateMap<Assignment, AssignmentVM>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<Attendance, AttendanceVM>()
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<Category, CategoryVM>();
            CreateMap<City, CityVM>();
            CreateMap<Permission, PermissionVM>();
            CreateMap<Role, RoleVM>();
            CreateMap<SessionAssignment, SessionAssignmentVM>();
            CreateMap<Setting, SettingVM>();
            CreateMap<User, UserVM>();
            CreateMap<User, TeacherVM>();

            CreateMap<Course, CourseVM>()
             .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Article, ArticleVM>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Event, EventVM>()
  .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<LoginMethod, LoginMethodVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<News, NewsVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<Notification, NotificationVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<Parent, ParentVM>()
.ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.StudentDetails.User.FirstName} {src.StudentDetails.User.LastName}"));

            CreateMap<PaymentHistory, PaymentHistoryVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<PermissionRole, PermissionRoleVM>()
.ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission.Name))
.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
.ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Permission.Routename))
.ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.Permission.PermissionType));

            CreateMap<Session, SessionVM>()
.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

            CreateMap<StudentDetails, StudentDetailsVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<TeacherResume, TeacherResumeVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<TicketMessage, TicketMessageVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<Ticket, TicketVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<UserGroup, UserGroupVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

            CreateMap<UserCourse, UserCourseVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
.ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<User, UserVM>()
.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
.ForMember(dest => dest.StudentDetailsId, opt => opt.MapFrom(src => src.StudentDetails.ID));



        }
    }

}
