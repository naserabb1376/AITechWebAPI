using AITechDATA.Domain;
using AITechWebAPI.ViewModels;
using AutoMapper;



namespace AITechWebAPI.Tools
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Group, TeacherGroupVM>();
        }
    }

}
