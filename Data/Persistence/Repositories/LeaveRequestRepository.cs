using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.Attendance;
using Data.Entities.HR;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence.Repositories
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext context) : base(context) { }
        public async Task<IEnumerable<LeaveRequest>> GetEmployeesOnLeaveByDateAsync(DateTime date)
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee) // DÒNG QUAN TRỌNG NHẤT: Join bảng Employee
                .Where(r => r.Status == "Approved"
                       && date.Date >= r.FromDate.Date
                       && date.Date <= r.ToDate.Date
                       && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingWithEmployeeAsync()
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee) // Bắt buộc phải có dòng này
                .Where(r => r.Status == "Pending" && !r.IsDeleted)
                .ToListAsync();
        }
    }
}