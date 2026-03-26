using Business.Dtos.AuthDtos;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstractions
{
    public interface IAuthService
    {
        Task<ResponseDto<LoginResponseDto>> Login(LoginRequestDto input);
        Task<ResponseDto<LoginResponseDto>> RefreshToken(string token);
    }
}
