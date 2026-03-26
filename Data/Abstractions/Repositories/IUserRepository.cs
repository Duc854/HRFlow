using Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmployeeIdAsync(int employeeId);
        Task<User?> GetByIdAsync(int id);
        Task<List<User?>> GetAllUserForCloneData();
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> IsExistsAsync(string username, string email);
        void Update(User user);
        void Add(User user);
    }
}
