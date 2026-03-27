using Data.Abstractions.Repositories;
using Data.Context;
using Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Persistence.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRoleRepository(ApplicationDbContext context) => _context = context;

        public void RemoveRange(IEnumerable<UserRole> entities)
        {
            _context.UserRoles.RemoveRange(entities);
        }
    }
}
