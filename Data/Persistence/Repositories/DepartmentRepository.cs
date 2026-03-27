using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.HR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Persistence.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Department>> GetAllActiveAsync()
        {
            return await _context.Departments
                .Where(d => !d.IsDeleted)
                .AsNoTracking() // Quan trọng: Tăng tốc độ vì chỉ để Read
                .ToListAsync();
        }

        public IQueryable<Department> GetDepartmentsQuery()
        {
            return _context.Departments.AsNoTracking();
        }
    }
}
