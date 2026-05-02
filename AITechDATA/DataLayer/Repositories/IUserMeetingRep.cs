using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IUserMeetingRep
    {
        Task<ListResultObject<UserMeeting>> GetAllUserMeetingsAsync(long userId=0,long meetingId=0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<UserMeeting>> GetUserMeetingByIdAsync(long UserMeetingId);


        Task<BitResultObject> AddUserMeetingsAsync(List<UserMeeting> UserMeetings);

        Task<BitResultObject> EditUserMeetingsAsync(List<UserMeeting> UserMeetings);

        Task<BitResultObject> RemoveUserMeetingsAsync(List<UserMeeting> UserMeetings);

        Task<BitResultObject> RemoveUserMeetingsAsync(List<long> UserMeetingIds);

        Task<BitResultObject> ExistUserMeetingAsync(long UserMeetingId);
    }
}