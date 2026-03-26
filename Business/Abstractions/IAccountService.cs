using Business.Dtos.AccountDtos;
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
    }
}
