using Data.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        // Danh sách các Repository
        IUserRepository Users { get; }
        IEmployeeRepository Employees { get; }
        IDepartmentRepository Departments { get; }
        IPositionRepository Positions { get; }
        IRoleRepository Roles { get; }
        IUserRoleRepository UserRoles { get; }



        // Quản lý SaveChanges
        Task<int> CommitAsync();

        // Quản lý Transaction cho các nghiệp vụ phức tạp (như Register)
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
