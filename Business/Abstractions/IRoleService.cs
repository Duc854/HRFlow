using Data.Entities.Identity;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface IRoleService
    {
        Task<ResponseDto<IEnumerable<string>>> GetAvailableRolesAsync();
    }
}
