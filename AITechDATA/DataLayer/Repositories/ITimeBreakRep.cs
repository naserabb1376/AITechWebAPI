using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ITimeBreakRep
    {
        Task<ListResultObject<TimeBreak>> GetAllTimeBreaksAsync(long timeFunctionId=0,int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<TimeBreak>> GetTimeBreakByIdAsync(long TimeBreakId);

        Task<BitResultObject> AddTimeBreakAsync(TimeBreak TimeBreak);

        Task<BitResultObject> EditTimeBreakAsync(TimeBreak TimeBreak);

        Task<BitResultObject> RemoveTimeBreakAsync(TimeBreak TimeBreak);

        Task<BitResultObject> RemoveTimeBreakAsync(long TimeBreakId);

        Task<BitResultObject> ExistTimeBreakAsync(long TimeBreakId);
    }
}