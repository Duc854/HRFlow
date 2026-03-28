using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.HR;

namespace Data.Abstractions.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<List<Employee>> GetAllAsync();
        IQueryable<Employee> GetEmployeesQuery();
        void Add(Employee? employee);
        Task<IEnumerable<Employee>> FindAsync(Expression<Func<Employee, bool>> expression);
    }
}
