using Data.Entities.Payroll;

namespace Data.Abstractions.Repositories
{
    public interface IPayrollRepository : IGenericRepository<EmployeePayroll>
    {
    }
}