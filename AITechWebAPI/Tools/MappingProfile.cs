using AiTech.Domains;
using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechWebAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.JSInterop;
using MTPermissionCenter.EFCore.Entities;
using NobatPlusDATA.Domain;



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
.ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
.ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"))
.ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
;

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
            CreateMap<MTPermissionCenter_Permission, PermissionVM>();
            CreateMap<Role, RoleVM>();
            CreateMap<SessionAssignment, SessionAssignmentVM>();
            CreateMap<Setting, SettingVM>();
            CreateMap<User, UserVM>();
            CreateMap<PreRegistration, PreRegistrationVM>();
            CreateMap<User, TeacherVM>();
            CreateMap<Meeting, MeetingVM>();
            CreateMap<SubmitForm, SubmitFormVM>();
            CreateMap<FormField, FormFieldVM>();

            CreateMap<Course, CourseVM>()
             .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Minutes, MinutesVM>()
            .ForMember(dest => dest.MeetingTitle, opt => opt.MapFrom(src => src.Meeting.MeetingTitle))
            .ForMember(dest => dest.MeetingOrganizer, opt => opt.MapFrom(src => src.Meeting.MeetingOrganizer))
            ;

            CreateMap<GroupChatMessage, GroupChatMessageVM>()
          .ForMember(dest => dest.RefMessageText, opt => opt.MapFrom(src => src.ReplyToMessage.Text ?? ""))
          .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
          .ForMember(dest => dest.GroupType, opt => opt.MapFrom(src => src.Group.GroupType))
          .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Group.Course.Title))
          .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Group.Teacher.FirstName} {src.Group.Teacher.LastName}"))
          .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.SenderUser.Username))
          .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.SenderUser.FirstName))
          .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.SenderUser.LastName))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.SenderUser.Email))
          .ForMember(dest => dest.NationalCode, opt => opt.MapFrom(src => src.SenderUser.NationalCode))

          ;

            CreateMap<Article, ArticleVM>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Event, EventVM>()
  .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<LoginMethod, LoginMethodVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<News, NewsVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<InterviewTime, InterviewTimeVM>()
.ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src => src.JobRequest.UniversityName))
.ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.JobRequest.CourseTitle))
.ForMember(dest => dest.EducationalLevel, opt => opt.MapFrom(src => src.JobRequest.EducationalLevel))
.ForMember(dest => dest.EducationStatus, opt => opt.MapFrom(src => src.JobRequest.EducationStatus))
.ForMember(dest => dest.LastAcademicLicense, opt => opt.MapFrom(src => src.JobRequest.LastAcademicLicense))
.ForMember(dest => dest.RequestedPosition, opt => opt.MapFrom(src => src.JobRequest.RequestedPosition))
.ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.JobRequest.BirthDate))
.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.JobRequest.FirstName))
.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.JobRequest.LastName))
.ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => src.JobRequest.FirstName))
.ForMember(dest => dest.NationalCode, opt => opt.MapFrom(src => src.JobRequest.NationalCode))
.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.JobRequest.Email))
.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.JobRequest.PhoneNumber))
;


            CreateMap<Notification, NotificationVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<Parent, ParentVM>()
.ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.StudentDetails.User.FirstName} {src.StudentDetails.User.LastName}"));

            CreateMap<PaymentHistory, PaymentHistoryVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<MTPermissionCenter_PermissionRole, PermissionRoleVM>()
.ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission.Name))
.ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Permission.Routename))
.ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.Permission.PermissionType));

            CreateMap<MTPermissionCenter_UserPermission, UserPermissionVM>()
.ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission.Name))
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
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
.ForMember(dest => dest.TeacherUserName, opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"))
.ForMember(dest => dest.TeacherUserId, opt => opt.MapFrom(src => src.TeacherId))
;

            CreateMap<UserGroup, UserGroupVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
.ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

            CreateMap<UserCourse, UserCourseVM>()
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
.ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
;

            CreateMap<FieldInForm, FieldInFormVM>()
.ForMember(dest => dest.FormKey, opt => opt.MapFrom(src => src.Form.FormKey))
.ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => src.Form.EntityName))
.ForMember(dest => dest.FormTitle, opt => opt.MapFrom(src => src.Form.Title))
.ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.FormField.FieldName))
.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FormField.DisplayName))
;

            CreateMap<User, UserVM>()
.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
.ForMember(dest => dest.StudentDetailsId, opt => opt.MapFrom(src => src.StudentDetails.ID))
.ForMember(dest => dest.EducationalGrade, opt => opt.MapFrom(src => src.EducationalBackground.EducationalGrade))
.ForMember(dest => dest.StudyField, opt => opt.MapFrom(src => src.EducationalBackground.StudyField))
;

            CreateMap<SMSMessage, SMSMessageVM>()
.ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
    ;

        }
    }

}
