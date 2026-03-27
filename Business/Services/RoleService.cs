using Business.Abstractions;
using Data.Abstractions;
using Data.Entities.Identity;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<ResponseDto<IEnumerable<string>>> GetAvailableRolesAsync()
        {
            try
            {
                var roles = await _unitOfWork.Roles.GetAllRolesAsync();
                var result = roles.Select(r => r.Name);
                return ResponseDto<IEnumerable<string>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<string>>.FailResult("500", ex.Message);
            }
        }
    }
}
