using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) => _context = context;


        public async Task<User?> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.EmployeeId == employeeId && !u.IsDeleted);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<List<User?>> GetAllUserForCloneData()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<bool> IsExistsAsync(string username, string email)
            => await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);

        public void Add(User user) => _context.Users.Add(user);
        public void Update(User user) => _context.Users.Update(user);
        public void Delete(User user) => _context.Users.Remove(user);

        public IQueryable<User> GetUsersQuery()
        {
            return _context.Users
                .Include(u => u.Employee)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsNoTracking();
        }

        public async Task<User?> GetUserWithFullProfileAsync(int userId)
        {
            // Dùng _context (hoặc tên biến DbContext trong GenericRepository của bạn)
            return await _context.Users
                // 1. Join từ User sang bảng Employee
                .Include(u => u.Employee)
                    // 2. Từ bảng Employee, Join tiếp sang bảng Department
                    .ThenInclude(e => e.Department)
                // 3. Quay lại Join từ User sang Employee một lần nữa...
                .Include(u => u.Employee)
                    // 4. ...để Join tiếp sang bảng Position
                    .ThenInclude(e => e.Position)
                // 5. Lấy ra User trùng ID (và chưa bị xóa mềm nếu hệ thống bạn có IsDeleted)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            // (Ghi chú: Nếu bảng User của bạn không có cột IsDeleted, hãy xóa đuôi && !u.IsDeleted đi nhé)
        }
    }
}