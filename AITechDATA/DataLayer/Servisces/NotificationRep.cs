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

namespace AITechDATA.DataLayer.Servisces
{
    public class NotificationRep : INotificationRep
    {
        private AiITechContext _context;

        public NotificationRep()
        {
            _context = DbTools.GetDbContext();
        }

        public async Task<BitResultObject> AddNotificationAsync(Notification notification)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                result.ID = notification.ID;
                _context.Entry(notification).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> EditNotificationAsync(Notification notification)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
                result.ID = notification.ID;
                _context.Entry(notification).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> ExistNotificationAsync(long notificationId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                result.Status = await _context.Notifications
                    .AsNoTracking()
                    .AnyAsync(x => x.ID == notificationId);
                result.ID = notificationId;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<ListResultObject<Notification>> GetAllNotificationsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "")
        {
            ListResultObject<Notification> results = new ListResultObject<Notification>();
            try
            {
                var query = _context.Notifications
                    .AsNoTracking()
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.Message) && x.Message.Contains(searchText))
                    );

                results.TotalCount = query.Count();
                results.PageCount = DbTools.GetPageCount(results.TotalCount, pageSize);
                results.Results = await query.OrderByDescending(x => x.CreateDate)
                    .ToPaging(pageIndex, pageSize)
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

        public async Task<RowResultObject<Notification>> GetNotificationByIdAsync(long notificationId)
        {
            RowResultObject<Notification> result = new RowResultObject<Notification>();
            try
            {
                result.Result = await _context.Notifications
                    .AsNoTracking()
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.ID == notificationId);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveNotificationAsync(Notification notification)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                result.ID = notification.ID;
                _context.Entry(notification).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return result;
        }

        public async Task<BitResultObject> RemoveNotificationAsync(long notificationId)
        {
            BitResultObject result = new BitResultObject();
            try
            {
                var notification = await GetNotificationByIdAsync(notificationId);
                result = await RemoveNotificationAsync(notification.Result);
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