using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface ISettingRep
    {
        Task<ListResultObject<Setting>> GetAllSettingsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<Setting>> GetSettingByIdAsync(long settingId);

        Task<BitResultObject> AddSettingAsync(Setting setting);

        Task<BitResultObject> EditSettingAsync(Setting setting);

        Task<BitResultObject> RemoveSettingAsync(Setting setting);

        Task<BitResultObject> RemoveSettingAsync(long settingId);

        Task<BitResultObject> ExistSettingAsync(long settingId);
    }
}