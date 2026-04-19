using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiTech.Domains;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IPaymentHistoryRep
    {
        Task<ListResultObject<PaymentHistory>> GetAllPaymentHistoriesAsync(long foreignkeyId = 0, string entityType = "", long UserId = 0,long DiscountId=0, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<PaymentHistory>> GetPaymentHistoryByIdAsync(long paymentHistoryId);

        Task<BitResultObject> AddPaymentHistoryAsync(PaymentHistory paymentHistory);

        Task<BitResultObject> EditPaymentHistoryAsync(PaymentHistory paymentHistory);

        Task<BitResultObject> RemovePaymentHistoryAsync(PaymentHistory paymentHistory);

        Task<BitResultObject> RemovePaymentHistoryAsync(long paymentHistoryId);

        Task<BitResultObject> ExistPaymentHistoryAsync(long paymentHistoryId);
    }
}