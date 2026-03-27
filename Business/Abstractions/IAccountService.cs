using Business.Dtos.UserDtos;
using Business.Dtos.UserDtos.AccountDtos;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface IAccountService
    {
        Task<ResponseDto<AccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto input);
        Task<ResponseDto<bool>> ToggleStatusAsync(int userId);
        Task<ResponseDto<string>> ResetPasswordAsync(int userId, string newPassword);
        Task<ResponseDto<IEnumerable<UserListDto>>> GetUserListAsync(string? search, string? roleName);
        Task<ResponseDto<bool>> UpdateUserRolesAsync(int userId, List<string> roleNames);
    }
}
