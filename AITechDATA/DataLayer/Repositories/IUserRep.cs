using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IUserRep
    {
        Task<ListResultObject<User>> GetAllUsersAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<User>> GetUserByIdAsync(long userId);

        Task<BitResultObject> AddUserAsync(User user);

        Task<BitResultObject> EditUserAsync(User user);

        Task<BitResultObject> RemoveUserAsync(User user);

        Task<BitResultObject> RemoveUserAsync(long userId);

        Task<BitResultObject> ExistUserAsync(long userId);
    }
}