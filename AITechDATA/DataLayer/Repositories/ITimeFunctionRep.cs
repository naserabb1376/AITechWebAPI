using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ITimeFunctionRep
    {
        Task<ListResultObject<TimeFunction>> GetAllTimeFunctionsAsync(long userId=0,string startDate="",string endDate ="",int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<TimeFunction>> GetTimeFunctionByIdAsync(long TimeFunctionId);

        Task<BitResultObject> AddTimeFunctionAsync(TimeFunction TimeFunction);

        Task<BitResultObject> EditTimeFunctionAsync(TimeFunction TimeFunction);

        Task<BitResultObject> RemoveTimeFunctionAsync(TimeFunction TimeFunction);

        Task<BitResultObject> RemoveTimeFunctionAsync(long TimeFunctionId);

        Task<BitResultObject> ExistTimeFunctionAsync(long TimeFunctionId);
    }
}