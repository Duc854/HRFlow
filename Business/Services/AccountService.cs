using Business.Abstractions;
using Business.Dtos.AccountDtos;
using Data.Abstractions;
using Data.Entities.Identity;
using Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public AccountService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseDto<AccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto input)
        {
            // 1. Kiểm tra Employee tồn tại
            var employee = await _unitOfWork.Employees.GetByIdAsync(input.EmployeeId);
            if (employee == null)
                return ResponseDto<AccountResponseDto>.FailResult("NotFound", "Không tìm thấy nhân viên này trong hệ thống");

            // 2. Check quan hệ 1-1: Employee này đã có Account chưa?
            var existingUser = await _unitOfWork.Users.GetByEmployeeIdAsync(input.EmployeeId);
            if (existingUser != null)
                return ResponseDto<AccountResponseDto>.FailResult("Conflict", $"Nhân viên {employee.FullName} đã được cấp tài khoản");

            // 3. Check trùng Username hoặc Email
            if (await _unitOfWork.Users.IsExistsAsync(input.Username, employee.Email))
                return ResponseDto<AccountResponseDto>.FailResult("Conflict", "Username hoặc Email đã tồn tại trên hệ thống");

            // 4. Khởi tạo User mới
            var user = new User
            {
                Username = input.Username,
                PasswordHash = _passwordHasher.HashPassword(input.Password),
                Email = employee.Email, // Lấy email từ hồ sơ nhân viên cho đồng bộ
                EmployeeId = input.EmployeeId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            // 5. Xử lý gán Roles (n-n)
            if (input.RoleIds != null && input.RoleIds.Any())
            {
                foreach (var roleId in input.RoleIds)
                {
                    user.UserRoles.Add(new UserRole { RoleId = roleId });
                }
            }

            _unitOfWork.Users.Add(user);
            await _unitOfWork.CommitAsync();

            return ResponseDto<AccountResponseDto>.SuccessResult(new AccountResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                EmployeeName = employee.FullName,
                IsActive = user.IsActive
            });
        }

        public async Task<ResponseDto<bool>> ToggleStatusAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return ResponseDto<bool>.FailResult("NotFound", "Không tìm thấy tài khoản");

            user.IsActive = !user.IsActive;
            user.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();

            return ResponseDto<bool>.SuccessResult(user.IsActive);
        }

        public async Task<ResponseDto<string>> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return ResponseDto<string>.FailResult("NotFound", "Tài khoản không tồn tại");

            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            user.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();

            return ResponseDto<string>.SuccessResult("Đặt lại mật khẩu thành công");
        }
    }
}