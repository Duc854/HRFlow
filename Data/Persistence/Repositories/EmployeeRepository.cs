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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context) => _context = context;

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }
    }
}
