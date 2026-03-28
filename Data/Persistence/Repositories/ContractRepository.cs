using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.Payroll;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractRepository(ApplicationDbContext context) => _context = context;

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                    .Include(c => c.Employee)
                        .ThenInclude(e => e.Department)
                    .Include(e => e.Employee)
                        .ThenInclude(e => e.Position)
                    .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contract?> GetActiveByEmployeeIdAsync(int employeeId)
        {
            return await _context.Contracts
                .Where(c => c.EmployeeId == employeeId && c.Status == "Active")
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefaultAsync();
        }

        public void Add(Contract contract) => _context.Contracts.Add(contract);
        public void Update(Contract contract) => _context.Contracts.Update(contract);
        public IQueryable<Contract> GetContractsQuery()
        {
            return _context.Contracts
                .Include(c => c.Employee)
                    .ThenInclude(e => e.Department)
                .Include(c => c.Employee)
                    .ThenInclude(e => e.Position)
                .AsNoTracking();
        }

        public async Task<IEnumerable<Contract>> GetAllContractsByEmployeeIdAsync(int employeeId)
        {
            // Lấy hết lịch sử, không lọc Active
            return await _context.Contracts
                .Where(c => c.EmployeeId == employeeId)
                .ToListAsync();
        }
    }
}
