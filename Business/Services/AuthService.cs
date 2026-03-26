using Business.Abstractions;
using Business.Dtos.AuthDtos;
using Data.Abstractions;
using Data.Entities.Identity;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenProvider _tokenProvider;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto<LoginResponseDto>> Login(LoginRequestDto input)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(input.Username);

            if (user == null || !_passwordHasher.Verify(input.Password, user.PasswordHash))
            {
                return ResponseDto<LoginResponseDto>.FailResult("InvalidCredentials", "Tài khoản hoặc mật khẩu không đúng");
            }

            // Tạo cặp Token mới
            string accessToken = _tokenProvider.GenerateAccessToken(user); // Tự lấy Role trong này
            string refreshToken = _tokenProvider.GenerateRefreshToken();

            // Cập nhật User
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();

            return ResponseDto<LoginResponseDto>.SuccessResult(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        public async Task<ResponseDto<LoginResponseDto>> RefreshToken(string token)
        {
            var user = await _unitOfWork.Users.GetByRefreshTokenAsync(token);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ResponseDto<LoginResponseDto>.FailResult("InvalidToken", "Phiên làm việc hết hạn");
            }

            string newAccessToken = _tokenProvider.GenerateAccessToken(user);
            string newRefreshToken = _tokenProvider.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();

            return ResponseDto<LoginResponseDto>.SuccessResult(new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        public async Task UpdatePasswordForCloneData()
        {
            // 1. Lấy toàn bộ User từ DB
            var users = await _unitOfWork.Users.GetAllUserForCloneData();

            // 2. Tạo mã hash chuẩn cho "123456"
            // Dùng BcryptPasswordHasher đã đăng ký trong DI
            string newHash = _passwordHasher.HashPassword("123456");

            foreach (var user in users)
            {
                user.PasswordHash = newHash;
                _unitOfWork.Users.Update(user);
            }

            // 3. Commit thay đổi
            await _unitOfWork.CommitAsync();
        }
    }
}
