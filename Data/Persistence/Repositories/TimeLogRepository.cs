using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.Attendance;
using Data.Entities.HR;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence.Repositories
{
    public class TimeLogRepository : GenericRepository<TimeLog>, ITimeLogRepository
    {
        public TimeLogRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<TimeLog>> GetLogsByMonthAsync(int employeeId, int month, int year)
        {
            return await _context.Set<TimeLog>()
                .Where(t => t.EmployeeId == employeeId && t.Date.Month == month && t.Date.Year == year && !t.IsDeleted)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        public async Task<TimeLog?> GetLogByDateAsync(int employeeId, DateTime date)
        {
            return await _context.Set<TimeLog>()
                .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.Date.Date == date.Date && !t.IsDeleted);
        }
    }
}