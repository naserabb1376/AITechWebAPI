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
    public interface IInterviewTimeRep
    {
        Task<ListResultObject<InterviewTime>> GetAllInterviewTimesAsync(long jobRequestId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<InterviewTime>> GetInterviewTimeByIdAsync(long InterviewTimeId);

        Task<BitResultObject> AddInterviewTimeAsync(InterviewTime InterviewTime);

        Task<BitResultObject> EditInterviewTimeAsync(InterviewTime InterviewTime);

        Task<BitResultObject> RemoveInterviewTimeAsync(InterviewTime InterviewTime);

        Task<BitResultObject> RemoveInterviewTimeAsync(long InterviewTimeId);

        Task<BitResultObject> ExistInterviewTimeAsync(long InterviewTimeId);
    }
}