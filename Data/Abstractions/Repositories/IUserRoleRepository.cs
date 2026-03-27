using Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Abstractions.Repositories
{
    public interface IUserRoleRepository
    {
        void RemoveRange(IEnumerable<UserRole> entities);
    }
}
