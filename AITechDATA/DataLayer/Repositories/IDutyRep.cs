using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AITechDATA.Domain;
using AITechDATA.ResultObjects;

namespace AITechDATA.DataLayer.Repositories
{
    public interface IDutyRep
    {
        Task<ListResultObject<Duty>> GetAllDutiesAsync(long userId = 0, long senderUserId = 0,int readState=2,int doneState=2, int pageIndex = 1, int pageSize = 20, string searchText = "",string sortQuery ="");

        Task<RowResultObject<Duty>> GetDutyByIdAsync(long DutyId);

        Task<BitResultObject> AddDutyAsync(Duty Duty);

        Task<BitResultObject> EditDutyAsync(Duty Duty);

        Task<BitResultObject> RemoveDutyAsync(Duty Duty);

        Task<BitResultObject> RemoveDutyAsync(long DutyId);

        Task<BitResultObject> ExistDutyAsync(long DutyId);

        // Chart

        Task<ListResultObject<EmployeePerformanceChartDto>> GetEmployeePerformanceChartAsync(DateTime? fromDate = null, DateTime? toDate = null, long userId = 0);
        Task<ListResultObject<DutyTrendChartDto>> GetDutyTrendChartAsync(DateTime? fromDate = null, DateTime? toDate = null, long userId = 0);
    }
}
