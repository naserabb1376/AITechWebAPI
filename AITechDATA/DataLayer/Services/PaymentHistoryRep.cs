using AiTech.Domains;
using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using AITechDATA.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITechDATA.DataLayer.Services
{
    public class PaymentHistoryRep : IPaymentHistoryRep
    {
        private AITechContext _context;

        public PaymentHistoryRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<BitResultObject> AddPaymentHistoryAsync(PaymentHistory paymentHistory)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                bool existsCompletedPayment = await _context.PaymentHistories.AnyAsync(p =>
                    p.UserId == paymentHistory.UserId &&
                    p.PaymentStatus &&
                    !string.IsNullOrEmpty(p.EntityType) &&
                    !string.IsNullOrEmpty(paymentHistory.EntityType) &&
                    p.EntityType.ToLower() == paymentHistory.EntityType.ToLower() &&
                    p.ForeignKeyId > 0 &&
                    p.ForeignKeyId == paymentHistory.ForeignKeyId);
                if (existsCompletedPayment && paymentHistory.PaymentStatus)
                {
                    throw new Exception("این عملیات پرداخت قبلا برای شما انجام شده است");
                }
                await _context.PaymentHistories.AddAsync(paymentHistory);
                await _context.SaveChangesAsync();
                result.ID = paymentHistory.ID;
                _context.Entry(paymentHistory).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditPaymentHistoryAsync(PaymentHistory paymentHistory)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PaymentHistories.Update(paymentHistory);
                await _context.SaveChangesAsync();
                result.ID = paymentHistory.ID;
                _context.Entry(paymentHistory).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistPaymentHistoryAsync(long paymentHistoryId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.PaymentHistories
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == paymentHistoryId);
                result.ID = paymentHistoryId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<PaymentHistory>> GetAllPaymentHistoriesAsync(long foreignkeyId = 0, string entityType = "", long UserId = 0, long DiscountId = 0, bool? paymentStatus = null, bool? hasDiscount = null, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<PaymentHistory> results = new ListResultObject<PaymentHistory>();
            try
            {
                var query = _context.PaymentHistories.Include(x=> x.User).Include(x => x.Discount).AsNoTracking();

                if (!string.IsNullOrWhiteSpace(entityType))
                {
                    var normalizedEntityType = entityType.Trim().ToLower();
                    query = query.Where(x => !string.IsNullOrEmpty(x.EntityType) && x.EntityType.ToLower() == normalizedEntityType);
                }

                if (foreignkeyId > 0)
                    query = query.Where(x => x.ForeignKeyId == foreignkeyId);
                if (UserId > 0)
                {
                    query = query.Where(x => x.UserId == UserId);
                }

                if (DiscountId > 0)
                {
                    query = query.Where(x => x.DiscountId == DiscountId);
                }

                if (paymentStatus.HasValue)
                {
                    query = query.Where(x => x.PaymentStatus == paymentStatus.Value);
                }

                if (hasDiscount.HasValue)
                {
                    query = hasDiscount.Value
                        ? query.Where(x => x.DiscountId.HasValue)
                        : query.Where(x => !x.DiscountId.HasValue);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(x =>
                        (x.Amount.ToString().Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.TargetObjName) && x.TargetObjName.Contains(searchText)) ||
                       (x.User != null && !string.IsNullOrEmpty(x.User.FirstName) && x.User.FirstName.Contains(searchText)) ||
                       (x.User != null && !string.IsNullOrEmpty(x.User.LastName) && x.User.LastName.Contains(searchText)) ||
                       (x.User != null && !string.IsNullOrEmpty(x.User.Username) && x.User.Username.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.EntityType) && x.EntityType.Contains(searchText)) ||
                       (x.Discount != null && !string.IsNullOrEmpty(x.Discount.DiscountCode) && x.Discount.DiscountCode.Contains(searchText))
                    );
                }

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                var rows = await query.OrderByDescending(x => x.PaymentDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .Include(x => x.Discount)
                    .ToListAsync();

                var groupIds = rows
                    .Where(x => !string.IsNullOrEmpty(x.EntityType) && x.EntityType.ToLower().Contains("group") && x.ForeignKeyId > 0)
                    .Select(x => x.ForeignKeyId)
                    .Distinct()
                    .ToList();

                if (groupIds.Any())
                {
                    var groupNames = await _context.Groups
                        .AsNoTracking()
                        .Where(x => groupIds.Contains(x.ID))
                        .Select(x => new { x.ID, x.Name })
                        .ToDictionaryAsync(x => x.ID, x => x.Name);

                    foreach (var row in rows.Where(x => !string.IsNullOrEmpty(x.EntityType) && x.EntityType.ToLower().Contains("group")))
                    {
                        if (groupNames.TryGetValue(row.ForeignKeyId, out var groupName))
                        {
                            row.TargetObjName = groupName;
                        }
                    }
                }

                results.Results = rows;
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return results;
        }

        public async Task<RowResultObject<PaymentHistory>> GetPaymentHistoryByIdAsync(long paymentHistoryId)
        {
            RowResultObject<PaymentHistory> result = new RowResultObject<PaymentHistory>();
            try
            {
                result.Result = await _context.PaymentHistories
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Discount)
                    .SingleOrDefaultAsync(x => x.ID == paymentHistoryId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

  

        public async Task<BitResultObject> RemovePaymentHistoryAsync(PaymentHistory paymentHistory)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.PaymentHistories.Remove(paymentHistory);
                await _context.SaveChangesAsync();
                result.ID = paymentHistory.ID;
                _context.Entry(paymentHistory).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemovePaymentHistoryAsync(long paymentHistoryId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var paymentHistory = await GetPaymentHistoryByIdAsync(paymentHistoryId);
                result = await RemovePaymentHistoryAsync(paymentHistory.Result);
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
