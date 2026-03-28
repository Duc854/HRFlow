using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Dtos.EmployeeDtos;
using Shared.Wrappers;

namespace HRFlow.Business.Interfaces
{
    public interface IProfileService
    {
        Task<ResponseDto<UserProfileDto>> GetMyProfileAsync(int userId);
        Task<ResponseDto<bool>> ChangeMyPasswordAsync(int userId, ChangePasswordDto dto);
        Task<ResponseDto<List<MyContractDto>>> GetMyContractsAsync(int employeeId);
    }
}