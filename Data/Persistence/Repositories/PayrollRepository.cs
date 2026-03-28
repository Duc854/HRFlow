using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.Payroll;

namespace Data.Persistence.Repositories
{
    public class PayrollRepository : GenericRepository<EmployeePayroll>, IPayrollRepository
    {
        public PayrollRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}