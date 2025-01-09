using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface INotificationRep
    {
        Task<ListResultObject<Notification>> GetAllNotificationsAsync(int pageIndex = 1, int pageSize = 20, string searchText = "");

        Task<RowResultObject<Notification>> GetNotificationByIdAsync(long notificationId);

        Task<BitResultObject> AddNotificationAsync(Notification notification);

        Task<BitResultObject> EditNotificationAsync(Notification notification);

        Task<BitResultObject> RemoveNotificationAsync(Notification notification);

        Task<BitResultObject> RemoveNotificationAsync(long notificationId);

        Task<BitResultObject> ExistNotificationAsync(long notificationId);
    }
}