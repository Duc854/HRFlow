using Data.Entities.HR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstractions.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllActiveAsync();
        IQueryable<Department> GetDepartmentsQuery();
        Task<Department?> GetByIdAsync(int id);
    }
}
