using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.CustomResponses;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IUserRep
    {
        Task<UserListCustomResponse<User>> GetAllUsersAsync(long groupId=0,long courseId =0, long sessionAssignmentId=0,long sessionId=0, long AddressId = 0, long RoleId = 0,string StudyField= "",string EducationalGrade ="", int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<UserRowCustomResponse<User>> GetUserByIdAsync(long userId);

        Task<RowResultObject<User>> AuthenticateAsync(string userName, string password, int loginType);

        Task<BitResultObject> AddUserAsync(User user);

        Task<BitResultObject> EditUserAsync(User user);

        Task<BitResultObject> RemoveUserAsync(User user);

        Task<BitResultObject> RemoveUserAsync(long userId);

        Task<BitResultObject> ExistUserAsync(string fieldValue, string fieldName,long userId = 0);
    }
}