using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IEducationalBackgroundRep
    {
        Task<ListResultObject<EducationalBackground>> GetAllEducationalBackgroundsAsync(long UserId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<EducationalBackground>> GetEducationalBackgroundByIdAsync(long EducationalBackgroundId);

        Task<BitResultObject> AddEducationalBackgroundAsync(EducationalBackground EducationalBackground);

        Task<BitResultObject> EditEducationalBackgroundAsync(EducationalBackground EducationalBackground);

        Task<BitResultObject> RemoveEducationalBackgroundAsync(EducationalBackground EducationalBackground);

        Task<BitResultObject> RemoveEducationalBackgroundAsync(long EducationalBackgroundId);

        Task<BitResultObject> ExistEducationalBackgroundAsync(long EducationalBackgroundId);
    }
}