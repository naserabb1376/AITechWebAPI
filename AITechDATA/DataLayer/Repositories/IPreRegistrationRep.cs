using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IPreRegistrationRep
    {
        Task<ListResultObject<PreRegistration>> GetAllPreRegistrationsAsync(long groupId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<PreRegistration>> GetPreRegistrationByIdAsync(long preRegistrationId);

        Task<BitResultObject> AddPreRegistrationAsync(PreRegistration preRegistration);

        Task<BitResultObject> EditPreRegistrationAsync(PreRegistration preRegistration);

        Task<BitResultObject> RemovePreRegistrationAsync(PreRegistration preRegistration);

        Task<BitResultObject> RemovePreRegistrationAsync(long preRegistrationId);

        Task<BitResultObject> ExistPreRegistrationAsync(long preRegistrationId);
    }
}