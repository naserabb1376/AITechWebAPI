using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IMeetingRep
    {
        Task<ListResultObject<Meeting>> GetAllMeetingsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<Meeting>> GetMeetingByIdAsync(long MeetingId);

        Task<BitResultObject> AddMeetingAsync(Meeting Meeting);

        Task<BitResultObject> EditMeetingAsync(Meeting Meeting);

        Task<BitResultObject> RemoveMeetingAsync(Meeting Meeting);

        Task<BitResultObject> RemoveMeetingAsync(long MeetingId);

        Task<BitResultObject> ExistMeetingAsync(long MeetingId);
    }
}