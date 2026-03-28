using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Dtos.EmployeeDtos;
using Data.Abstractions;
using Data.Entities.Payroll;
using HRFlow.Business.Interfaces;
using Shared.Wrappers; // Dùng chuẩn ResponseDto mới

namespace HRFlow.Business.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<UserProfileDto>> GetMyProfileAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWithFullProfileAsync(userId);

            if (user == null || user.Employee == null)
            {
                // DÙNG HÀM STATIC THEO CHUẨN MỚI
                return ResponseDto<UserProfileDto>.FailResult("NOT_FOUND", "Không tìm thấy hồ sơ cá nhân của người dùng này.");
            }

            var dto = new UserProfileDto
            {
                EmployeeId = user.EmployeeId.Value,
                // FIX LỖI CRASH: Fomat Id (int) thành chuỗi có độ dài cố định, vd: EMP00001
                EmployeeCode = $"EMP{user.Employee.Id:D5}",
                FullName = user.Employee.FullName,
                DoB = user.Employee.DoB,
                Gender = user.Employee.Gender,
                Address = user.Employee.Address,
                Phone = user.Employee.Phone,
                Email = user.Email,
                DepartmentName = user.Employee.Department?.Name ?? "Chưa phân bổ",
                PositionName = user.Employee.Position?.Name ?? "Chưa phân bổ",
                JoinDate = user.Employee.JoinDate
            };

            // DÙNG HÀM STATIC SUCCESS
            return ResponseDto<UserProfileDto>.SuccessResult(dto);
        }

        public async Task<ResponseDto<bool>> ChangeMyPasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null || user.IsDeleted || !user.IsActive)
            {
                return ResponseDto<bool>.FailResult("INVALID_ACCOUNT", "Tài khoản không tồn tại hoặc đã bị khóa.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return ResponseDto<bool>.FailResult("WRONG_PASSWORD", "Mật khẩu cũ không chính xác!");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedDate = DateTime.Now;

            _unitOfWork.Users.Update(user);

            var result = await _unitOfWork.CommitAsync();

            if (result > 0)
            {
                return ResponseDto<bool>.SuccessResult(true);
            }

            return ResponseDto<bool>.FailResult("DB_ERROR", "Lỗi khi lưu dữ liệu vào hệ thống.");
        }

        public async Task<ResponseDto<List<MyContractDto>>> GetMyContractsAsync(int employeeId)
        {
            // Gọi hàm lấy TOÀN BỘ lịch sử hợp đồng
            var contracts = await _unitOfWork.Contracts.GetAllContractsByEmployeeIdAsync(employeeId);

            if (contracts == null || !contracts.Any())
            {
                return ResponseDto<List<MyContractDto>>.SuccessResult(new List<MyContractDto>());
            }

            var data = contracts
                .Select(c => new MyContractDto
                {
                    Id = c.Id,
                    ContractNumber = c.ContractNumber,
                    ContractType = c.ContractType,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    BasicSalary = c.BasicSalary,
                    // (Tuỳ chọn) Bạn có thể bổ sung trường Status vào MyContractDto để FE hiển thị cái nào đang Active, cái nào đã Hết hạn
                })
                .OrderByDescending(c => c.StartDate) // Đẩy hợp đồng mới nhất (thường là Active) lên đầu
                .ToList();

            return ResponseDto<List<MyContractDto>>.SuccessResult(data);
        }
    }
}