using AiTech.Domains;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AITechDATA.DataLayer.Services
{
    public class PaymentInstallmentRep : IPaymentInstallmentRep
    {
        private AITechContext _context;

        public PaymentInstallmentRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddPaymentInstallmentAsync(PaymentInstallment PaymentInstallment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.PaymentInstallments.AddAsync(PaymentInstallment);
                await RefereshPaymentStatusAsync(PaymentInstallment.PaymentHistoryId);
                await _context.SaveChangesAsync();
                result.ID = PaymentInstallment.ID;
                _context.Entry(PaymentInstallment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }
        public async Task<BitResultObject> EditPaymentInstallmentAsync(PaymentInstallment PaymentInstallment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PaymentInstallments.Update(PaymentInstallment);
                await RefereshPaymentStatusAsync(PaymentInstallment.PaymentHistoryId);
                await _context.SaveChangesAsync();
                result.ID = PaymentInstallment.ID;
                _context.Entry(PaymentInstallment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistPaymentInstallmentAsync(long PaymentInstallmentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.PaymentInstallments
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == PaymentInstallmentId);
                result.ID = PaymentInstallmentId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<PaymentInstallment>> GetAllPaymentInstallmentsAsync(long paymentHistoryId = 0, long UserId = 0, int payState = 2, int alowState = 2, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<PaymentInstallment> results = new ListResultObject<PaymentInstallment>();
            try
            {
                var query = _context.PaymentInstallments.Include(x=> x.PaymentHistory).ThenInclude(x => x.User).AsNoTracking();

                if (paymentHistoryId > 0)
                    query = query.Where(x => x.PaymentHistoryId == paymentHistoryId);
                if (UserId > 0)
                {
                    query = query.Where(x => x.PaymentHistory.UserId == UserId);
                }
                if (payState < 2)
                {
                    query = query.Where(x => x.IsPaid == Convert.ToBoolean(payState));
                }
                if (alowState < 2)
                {
                    query = query.Where(x => x.PayAllowed == Convert.ToBoolean(alowState));
                }

                query = query.Where(x =>
                        (x.Amount.ToString().Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.InstallmentNumber.ToString()) && x.InstallmentNumber.ToString().Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.PaymentHistory.TargetObjName) && x.PaymentHistory.TargetObjName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.PaymentHistory.EntityType) && x.PaymentHistory.EntityType.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.PaymentHistory.User.FirstName) && x.PaymentHistory.User.FirstName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.PaymentHistory.User.LastName) && x.PaymentHistory.User.LastName.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.PaidDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<PaymentInstallment>> GetPaymentInstallmentByIdAsync(long PaymentInstallmentId)
        {
            RowResultObject<PaymentInstallment> result = new RowResultObject<PaymentInstallment>();
            try
            {
                result.Result = await _context.PaymentInstallments
                    .AsNoTracking()
                    .Include(x => x.PaymentHistory).ThenInclude(x => x.User)
                    .SingleOrDefaultAsync(x => x.ID == PaymentInstallmentId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

  

        public async Task<BitResultObject> RemovePaymentInstallmentAsync(PaymentInstallment PaymentInstallment)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PaymentInstallments.Remove(PaymentInstallment);
                await RefereshPaymentStatusAsync(PaymentInstallment.PaymentHistoryId);
                await _context.SaveChangesAsync();
                result.ID = PaymentInstallment.ID;
                _context.Entry(PaymentInstallment).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePaymentInstallmentAsync(long PaymentInstallmentId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var PaymentInstallment = await GetPaymentInstallmentByIdAsync(PaymentInstallmentId);
                result = await RemovePaymentInstallmentAsync(PaymentInstallment.Result);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RefereshPaymentStatusAsync(long PaymentHistoryId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var PaymentHistory = await _context.PaymentHistories.Include(x=> x.PaymentInstallments).FirstOrDefaultAsync(x=> x.ID == PaymentHistoryId);
                PaymentHistory.PaymentStatus = !(PaymentHistory.PaymentInstallments.Any(x => !x.IsPaid));
                _context.PaymentHistories.Update(PaymentHistory);
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }
    }
}
