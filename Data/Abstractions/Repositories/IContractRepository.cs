using Data.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstractions.Repositories
{
    public interface IContractRepository
    {
        Task<Contract?> GetByIdAsync(int id);
        Task<Contract?> GetActiveByEmployeeIdAsync(int employeeId);
        void Add(Contract contract);
        void Update(Contract contract);
        IQueryable<Contract> GetContractsQuery();
    }
}
