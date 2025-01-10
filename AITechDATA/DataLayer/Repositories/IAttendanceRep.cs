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
        Task<ListResultObject<Attendance>> GetAllAttendancesAsync(int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Attendance>> GetAttendanceByIdAsync(long attendanceId);

        Task<BitResultObject> AddAttendanceAsync(Attendance attendance);

        Task<BitResultObject> EditAttendanceAsync(Attendance attendance);

        Task<BitResultObject> RemoveAttendanceAsync(Attendance attendance);

        Task<BitResultObject> RemoveAttendanceAsync(long attendanceId);

        Task<BitResultObject> ExistAttendanceAsync(long attendanceId);
    }
}