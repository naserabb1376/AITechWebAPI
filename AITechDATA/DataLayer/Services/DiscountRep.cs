using AITechDATA.DataLayer.Repositories;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;
using Microsoft.EntityFrameworkCore;
using AITechDATA.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace AITechDATA.DataLayer.Services
{
    public class DiscountRep : IDiscountRep
    {
        private AITechContext _context;

        public DiscountRep(AITechContext context)
        {
            _context = context;
        }

        public async Task<RowResultObject<Discount>> AddDiscountAsync(Discount Discount)
        {
            RowResultObject<Discount> result = new RowResultObject<Discount>();
            try
            {
                bool exists = await _context.Discounts.AnyAsync(x=> x.DiscountCode.ToLower() == Discount.DiscountCode.ToLower() && x.IsActive && DateTime.Now >= x.ExpireDate );
                if (exists)
                {
                    throw new Exception("این کد تخفیف در سیستم فعال است");
                }
                await _context.Discounts.AddAsync(Discount);
                await _context.SaveChangesAsync();
                result.Result = Discount;
                _context.Entry(Discount).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<RowResultObject<Discount>> EditDiscountAsync(Discount Discount)
        {
            RowResultObject<Discount> result = new RowResultObject<Discount>();
            try
            {
                bool exists = await _context.Discounts.AnyAsync(x => x.DiscountCode.ToLower() == Discount.DiscountCode.ToLower() && x.ID != Discount.ID && x.IsActive && DateTime.Now >= x.ExpireDate);
                if (exists)
                {
                    throw new Exception("این کلید فرم در سیستم فعال است");
                }
                _context.Discounts.Update(Discount);
                await _context.SaveChangesAsync();
                result.Result = Discount;
                _context.Entry(Discount).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistDiscountAsync(string existType, string keyValue,long userId)
        {
            BitResultObject result = new BitResultObject();
         
                long discountId = 0;
                try
                {
                    switch (existType.ToLower().Trim())
                    {
                        case "id":
                        default:
                            {
                                var theDiscount = await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(x => x.ID == long.Parse(keyValue)) ?? new Discount();
                                discountId = theDiscount.ID;
                                break;
                            }
                        case "validatecode":
                            {
                                var theDiscount = await _context.Discounts.Include(x=> x.PaymentHistories).AsNoTracking().FirstOrDefaultAsync(x => x.DiscountCode.ToLower() == keyValue.ToLower() && x.IsActive && x.DiscountMaxUsage > x.PaymentHistories.Count(p=> p.UserId == userId) && x.ExpireDate >= DateTime.Now.ToShamsi()) ?? new Discount();
                                discountId = theDiscount.ID;
                                break;
                            }
                       
                    }
                    result.ID = discountId;
                    result.Status = discountId > 0;
                 
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Discount>> GetAllDiscountsAsync(
      string entityName = "", long foreignKeyId = 0, long creatorId = 0,
      int pageIndex = 1, int pageSize = 20,
      string searchText = "", string sortQuery = "")
        {
            ListResultObject<Discount> results = new ListResultObject<Discount>();

            try
            {
                var query = _context.Discounts.Include(x => x.DiscountTargets).Include(x => x.PaymentHistories).AsNoTracking().AsQueryable();

                // شرط‌های دینامیک فقط در صورت معتبر بودن

                if (creatorId > 0)
                    query = query.Where(x => x.CreatorId == creatorId);

                if (foreignKeyId > 0)
                    query = query.Where(x => x.ForeignKeyId == foreignKeyId);

                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(x => x.EntityName == entityName);


                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(x =>
                        (!string.IsNullOrEmpty(x.DiscountCode) && x.DiscountCode.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.EntityName) && x.EntityName.Contains(searchText)) ||
                        (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(searchText))
                    );
                }

                results.TotalCount = await query.CountAsync();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);

                results.Results = await query
                    .OrderBy(x => x.CreateDate)
                    .SortBy(sortQuery)
                    .ToPaging(pageIndex, pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                results.Status = false;
                results.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }

            return results;
        }


      

        public async Task<RowResultObject<Discount>> GetDiscountByIdAsync(long DiscountId)
        {
            RowResultObject<Discount> result = new RowResultObject<Discount>();
            try
            {
                result.Result = await _context.Discounts
                    .Include(x=> x.DiscountTargets).Include(x=> x.PaymentHistories).AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ID == DiscountId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

     

        public async Task<BitResultObject> RemoveDiscountAsync(Discount Discount)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Discounts.Remove(Discount);
                await _context.SaveChangesAsync();
                result.ID = Discount.ID;
                _context.Entry(Discount).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveDiscountAsync(long DiscountId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var Discount = await GetDiscountByIdAsync(DiscountId);
                result = await RemoveDiscountAsync(Discount.Result);
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