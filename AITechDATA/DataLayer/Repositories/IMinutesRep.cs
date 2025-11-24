using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IMinutesRep
    {
        Task<ListResultObject<Minutes>> GetAllMinutesAsync(long meetingId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<Minutes>> GetMinutesByIdAsync(long MinutesId);

        Task<BitResultObject> AddMinutesAsync(Minutes Minutes);

        Task<BitResultObject> EditMinutesAsync(Minutes Minutes);

        Task<BitResultObject> RemoveMinutesAsync(Minutes Minutes);

        Task<BitResultObject> RemoveMinutesAsync(long MinutesId);

        Task<BitResultObject> ExistMinutesAsync(long MinutesId);
    }
}