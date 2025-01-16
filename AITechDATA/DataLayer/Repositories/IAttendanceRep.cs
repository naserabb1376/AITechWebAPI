using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IAttendanceRep
    {
        Task<ListResultObject<Attendance>> GetAllAttendancesAsync(long userId = 0, long sessionId=0,int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Attendance>> GetAttendanceByIdAsync(long attendanceId);

        Task<BitResultObject> AddAttendancesAsync(List<Attendance> attendances);

        Task<BitResultObject> EditAttendancesAsync(List<Attendance> attendances);

        Task<BitResultObject> RemoveAttendancesAsync(List<Attendance> attendances);

        Task<BitResultObject> RemoveAttendancesAsync(List<long> attendanceIds);

        Task<BitResultObject> ExistAttendanceAsync(long attendanceId);
    }
}