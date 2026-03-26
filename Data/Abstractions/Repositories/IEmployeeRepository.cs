using Data.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstractions.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<List<Employee>> GetAllAsync();
    }
}
