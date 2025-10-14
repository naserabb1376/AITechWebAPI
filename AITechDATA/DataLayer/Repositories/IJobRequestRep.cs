using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IJobRequestRep
    {
        Task<ListResultObject<JobRequest>> GetAllJobRequestsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<JobRequest>> GetJobRequestByIdAsync(long JobRequestId);

        Task<BitResultObject> AddJobRequestAsync(JobRequest JobRequest);

        Task<BitResultObject> EditJobRequestAsync(JobRequest JobRequest);
        Task<BitResultObject> ChangeCheckStatus(long JobRequestId,string CheckStatus);

        Task<BitResultObject> RemoveJobRequestAsync(JobRequest JobRequest);

        Task<BitResultObject> RemoveJobRequestAsync(long JobRequestId);

        Task<BitResultObject> ExistJobRequestAsync(long JobRequestId);
    }
}