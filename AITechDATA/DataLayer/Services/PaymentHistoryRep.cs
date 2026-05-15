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
using System.Xml;

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
                bool ExistPayment = await _context.PreRegistrations.AnyAsync(p=>
                    !string.IsNullOrEmpty(p.EntityType) &&
                    !string.IsNullOrEmpty(paymentHistory.EntityType) &&
                    p.EntityType.ToLower() == paymentHistory.EntityType.ToLower() &&
                    p.ForeignKeyId > 0 &&
                    p.ForeignKeyId == paymentHistory.ForeignKeyId);
                if (ExistPayment)
                {
                    throw new Exception("این عملیات پرداخت قبلا برای شما انجام شده است");
                }
                if (paymentHistory.IsInstallment)
                {
                    var PaymentInstallmentStandardCountRow = await _context.Settings.FirstOrDefaultAsync(x=> x.Key.ToLower() == "paymentinstallmentstandardcount");
                    var PaymentInstallmentStandardCount = int.Parse(PaymentInstallmentStandardCountRow.Value);
                    var PaymentInstallmentStandardDurationRow = await _context.Settings.FirstOrDefaultAsync(x=> x.Key.ToLower() == "paymentinstallmentstandardduration");
                    var PaymentInstallmentStandardDuration = int.Parse(PaymentInstallmentStandardDurationRow.Value);
                    var FirstPaymentInstallmentStandardPercentRow = await _context.Settings.FirstOrDefaultAsync(x => x.Key.ToLower() == "firstpaymentinstallmentstandardpercent");
                    var FirstPaymentInstallmentStandardPercent = int.Parse(FirstPaymentInstallmentStandardPercentRow.Value);

                    var paymentInstallments = new List<PaymentInstallment>();
                    var firstInstallmenAmount = Math.Round(paymentHistory.Amount * FirstPaymentInstallmentStandardPercent / 100, 0);
                    for (int i = 0; i < PaymentInstallmentStandardCount; i++)
                    {
                        var installmentNum = ++i;
                        paymentInstallments.Add(new PaymentInstallment()
                        {
                            CreateDate = DateTime.Now.ToShamsi(),
                            UpdateDate = DateTime.Now.ToShamsi(),
                            IsActive = true,
                            InstallmentNumber = installmentNum,
                            PayAllowed = installmentNum == 1,
                            DueDate = DateTime.Now.AddDays(i * PaymentInstallmentStandardDuration),
                            Amount = installmentNum == 1 ? firstInstallmenAmount : ((paymentHistory.Amount - firstInstallmenAmount) / (PaymentInstallmentStandardCount - 1)),
                            IsPaid = false,
                            
                        });
                    }
                    paymentHistory.PaymentInstallments=paymentInstallments;
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

        public async Task<ListResultObject<PaymentHistory>> GetAllPaymentHistoriesAsync(long foreignkeyId = 0, string entityType = "", long UserId = 0, long DiscountId = 0, int payState = 2, int pageIndex = 1, int pageSize = 20, string searchText = "", string sortQuery = "")
        {
            ListResultObject<PaymentHistory> results = new ListResultObject<PaymentHistory>();
            try
            {
                var query = _context.PaymentHistories.Include(x=> x.PaymentInstallments).Include(x=> x.User).AsNoTracking();

                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(x => x.EntityType == entityType);

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

                if (payState < 2)
                {
                    bool status = Convert.ToBoolean(payState);

                    query = query.Where(x =>
                        x.PaymentStatus == status &&
                        (
                            !x.IsInstallment || // اگر قسطی نیست
                            x.PaymentInstallments.All(i => i.IsPaid == status) // اگر قسطی است، همه اقساط باید همین وضعیت را داشته باشند
                        )
                    );
                }

                query = query.Where(x =>
                        (x.Amount.ToString().Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.TargetObjName) && x.TargetObjName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.User.FirstName) && x.User.FirstName.Contains(searchText)) ||
                       (!string.IsNullOrEmpty(x.User.LastName) && x.User.LastName.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.PaymentDate)
                     .SortBy(sortQuery).ToPaging(pageIndex, pageSize)
                    .Include(x => x.User)
                    .ToListAsync();
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
                    .Include(x=> x.PaymentInstallments).Include(x => x.User)
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
