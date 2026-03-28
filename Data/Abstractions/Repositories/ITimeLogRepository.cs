using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Attendance;
using Data.Entities.HR;

namespace Data.Abstractions.Repositories
{
    public interface ITimeLogRepository : IGenericRepository<TimeLog>
    {
        Task<IEnumerable<TimeLog>> GetLogsByMonthAsync(int employeeId, int month, int year);
        Task<TimeLog?> GetLogByDateAsync(int employeeId, DateTime date);
    }
}