using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiTech.Domains;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IPaymentInstallmentRep
    {
        Task<ListResultObject<PaymentInstallment>> GetAllPaymentInstallmentsAsync(long paymentHistoryId = 0, long UserId = 0, int payState = 2, int alowState = 2, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "");

        Task<RowResultObject<PaymentInstallment>> GetPaymentInstallmentByIdAsync(long PaymentInstallmentId);
        Task<BitResultObject> AddPaymentInstallmentAsync(PaymentInstallment PaymentInstallment);

        Task<BitResultObject> EditPaymentInstallmentAsync(PaymentInstallment PaymentInstallment);

        Task<BitResultObject> RemovePaymentInstallmentAsync(PaymentInstallment PaymentInstallment);

        Task<BitResultObject> RemovePaymentInstallmentAsync(long PaymentInstallmentId);

        Task<BitResultObject> ExistPaymentInstallmentAsync(long PaymentInstallmentId);
    }
}