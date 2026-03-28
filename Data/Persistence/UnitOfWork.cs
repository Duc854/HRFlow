using System;
using System.Threading.Tasks;
using Data.Abstractions;
using Data.Abstractions.Repositories;
using Data.Context; // Nhớ đảm bảo ApplicationDbContext nằm đúng namespace này
using Data.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Business.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IUserRepository? _users;
        private IEmployeeRepository? _employees;
        private IDepartmentRepository? _department;
        private IPositionRepository? _position;
        private IRoleRepository? _role;
        private IUserRoleRepository? _userRoles;
        private IContractRepository? _contracts;

        // --- 2 BIẾN MỚI THÊM ---
        private ITimeLogRepository? _timeLogs;
        private ILeaveRequestRepository? _leaveRequests;
        private IPayrollRepository? _payrolls;
        public IPayrollRepository Payrolls => _payrolls ??= new PayrollRepository(_context);

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IEmployeeRepository Employees => _employees ??= new EmployeeRepository(_context);
        public IDepartmentRepository Departments => _department ??= new DepartmentRepository(_context);
        public IPositionRepository Positions => _position ??= new PositionRepository(_context);
        public IRoleRepository Roles => _role ??= new RoleRepository(_context);
        public IUserRoleRepository UserRoles => _userRoles ??= new UserRoleRepository(_context);
        public IContractRepository Contracts => _contracts ??= new ContractRepository(_context);

        // --- 2 PROPERTY MỚI THÊM ---
        public ITimeLogRepository TimeLogs => _timeLogs ??= new TimeLogRepository(_context);
        public ILeaveRequestRepository LeaveRequests => _leaveRequests ??= new LeaveRequestRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null) await _transaction.CommitAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}