using Business.Abstractions;
using Data.Abstractions;
using Data.Entities.Identity;
using Shared.Wrappers;
using Microsoft.EntityFrameworkCore;
using Business.Dtos.UserDtos.AccountDtos;
using Business.Dtos.UserDtos;
using System.Data;

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
                return ResponseDto<AccountResponseDto>.FailResult("Conflict", "Username hoặc Email của nhân viên đã được đăng kí");

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

            var defaultRole = await _unitOfWork.Roles.GetRolesByNameAsync("Employee");
            if (defaultRole == null)
            {
                // Case này xảy ra nếu ní chưa Seed Data bảng Roles
                return ResponseDto<AccountResponseDto>.FailResult("500", "Hệ thống chưa cấu hình quyền 'Employee'. Vui lòng liên hệ Admin.");
            }

            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = defaultRole.Id });

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

        public async Task<ResponseDto<IEnumerable<UserListDto>>> GetUserListAsync(string? search, string? roleName)
        {
            try
            {
                var query = _unitOfWork.Users.GetUsersQuery();

                // 1. Tìm kiếm theo Username hoặc Tên nhân viên
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(u => u.Username.Contains(search)
                                        || u.Employee.FullName.Contains(search));
                }

                // 2. Lọc theo Role
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName));
                }

                var users = await query.ToListAsync();

                // 3. Mapping ra DTO
                var result = users.Select(u => new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.Employee?.FullName ?? "N/A",
                    Email = u.Email,
                    Status = u.IsActive ? "Acitve" : "Deactive",
                    CreatedAt = u.CreatedDate,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                });

                return ResponseDto<IEnumerable<UserListDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<UserListDto>>.FailResult("500", ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> UpdateUserRolesAsync(int userId, List<string> roleNames)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = await _unitOfWork.Users.GetUsersQuery()
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return ResponseDto<bool>.FailResult("404", "Không tìm thấy tài khoản");

                if (roleNames == null || !roleNames.Any())
                {
                    return ResponseDto<bool>.FailResult("400", "Danh sách quyền không được để trống.");
                }

                var roles = (await _unitOfWork.Roles.GetRolesByNamesAsync(roleNames)).ToList();

                if (roles.Count != roleNames.Distinct().Count())
                {
                    return ResponseDto<bool>.FailResult("400", "Một hoặc nhiều quyền gửi lên không tồn tại trong hệ thống.");
                }

                _unitOfWork.UserRoles.RemoveRange(user.UserRoles);

                foreach (var role in roles)
                {
                    user.UserRoles.Add(new UserRole { UserId = userId, RoleId = role.Id });
                }

                await _unitOfWork.CommitTransactionAsync();
                return ResponseDto<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResponseDto<bool>.FailResult("500", ex.Message);
            }
        }
    }
}