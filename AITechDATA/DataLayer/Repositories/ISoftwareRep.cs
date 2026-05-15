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
    public interface ISoftwareRep
    {
        Task<SoftwareListCustomResponse<Software>> GetAllSoftwaresAsync(long categoryId = 0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<SoftwareRowCustomResponse<Software>> GetSoftwareByIdAsync(long SoftwareId);

        Task<BitResultObject> AddSoftwareAsync(Software Software);

        Task<BitResultObject> EditSoftwareAsync(Software Software);

        Task<BitResultObject> RemoveSoftwareAsync(Software Software);

        Task<BitResultObject> RemoveSoftwareAsync(long SoftwareId);

        Task<BitResultObject> ExistSoftwareAsync(long SoftwareId);
    }
}
