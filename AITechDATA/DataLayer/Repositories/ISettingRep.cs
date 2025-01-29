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
    public interface ISettingRep
    {
        Task<SettingListCustomResponse<Setting>> GetAllSettingsAsync(long ParentId = 0,string key="", int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<SettingRowCustomResponse<Setting>> GetSettingByIdAsync(long settingId);

        Task<BitResultObject> AddSettingAsync(Setting setting);

        Task<BitResultObject> EditSettingAsync(Setting setting);

        Task<BitResultObject> RemoveSettingAsync(Setting setting);

        Task<BitResultObject> RemoveSettingAsync(long settingId);

        Task<BitResultObject> ExistSettingAsync(long settingId);
    }
}