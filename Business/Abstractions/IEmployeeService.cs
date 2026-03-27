using Business.Dtos.EmployeeDtos;
using Shared.Models;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface IEmployeeService
    {
        Task<ResponseDto<IEnumerable<EmployeeListDto>>> GetEmployeeListAsync(UserIdentity identity, string? search, int? deptId, string status, bool? joinDateDes,
            bool isDeleted);
        Task<ResponseDto<int>> CreateEmployeeAsync(UserIdentity identity, CreateEmployeeDto dto);
        Task<ResponseDto<bool>> UpdateEmployeeAsync(UserIdentity identity, int id, UpdateEmployeeDto dto);
        Task<ResponseDto<bool>> SoftDeleteEmployeeAsync(UserIdentity identity, int id);
        Task<ResponseDto<EmployeeListDto>> GetEmployeeDetailAsync(UserIdentity identity, int id);
        Task<ResponseDto<bool>> PromoteToManagerAsync(UserIdentity identity, int employeeId);
    }
}
