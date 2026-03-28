using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.Attendance;
using Data.Entities.HR;

namespace Data.Persistence.Repositories
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext context) : base(context) { }
    }
}