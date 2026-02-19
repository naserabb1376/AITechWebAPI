using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using MTPermissionCenter.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IDiscountTargetRep
    {
        Task<ListResultObject<DiscountTarget>> GetAllDiscountTargetsAsync(long DiscountId = 0, long TargetId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<DiscountTarget>> GetDiscountTargetByIdAsync(long DiscountTargetId);

        Task<BitResultObject> AddDiscountTargetsAsync(List<DiscountTarget> DiscountTargets);

        Task<BitResultObject> EditDiscountTargetsAsync(List<DiscountTarget> DiscountTargets);

        Task<BitResultObject> RemoveDiscountTargetsAsync(List<DiscountTarget> DiscountTargets);

        Task<BitResultObject> RemoveDiscountTargetsAsync(List<long> DiscountTargetIds);

        Task<BitResultObject> ExistDiscountTargetAsync(long DiscountTargetId);
    }
}