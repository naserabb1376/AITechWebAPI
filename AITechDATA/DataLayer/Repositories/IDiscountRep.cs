using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IDiscountRep
    {
        Task<ListResultObject<Discount>> GetAllDiscountsAsync(string entityName = "", long foreignKeyId = 0, long creatorId = 0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");
        Task<RowResultObject<Discount>> GetDiscountByIdAsync(long DiscountId);
        Task<RowResultObject<Discount>> AddDiscountAsync(Discount Discount);
        Task<RowResultObject<Discount>> EditDiscountAsync(Discount Discount);
        Task<BitResultObject> RemoveDiscountAsync(Discount Discount);
        Task<BitResultObject> RemoveDiscountAsync(long DiscountId);
        Task<BitResultObject> ExistDiscountAsync(string existType,string keyValue,long userId, long? foreignkeyId, string? entityName, long? roleId);
    }
}