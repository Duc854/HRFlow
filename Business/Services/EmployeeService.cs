using Business.Abstractions;
using Business.Dtos.EmployeeDtos;
using Data.Abstractions;
using Data.Abstractions.Repositories;
using Data.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<ResponseDto<IEnumerable<EmployeeListDto>>> GetEmployeeListAsync(
            UserIdentity identity,
            string? search,
            int? deptId,
            string status,
            bool? joinDateDes,
            bool isDeleted)
        {
            // Check quyền cao nhất (Admin/Director)
            var isHighestPermission = identity.IsAdmin || identity.IsDirector;

            try
            {
                // 1. Lấy Query từ UnitOfWork
                var query = _unitOfWork.Employees.GetEmployeesQuery();

                bool showDeleted = isHighestPermission && isDeleted;
                query = query.IgnoreQueryFilters().Where(e => e.IsDeleted == showDeleted);

                // 2. Phân quyền dữ liệu (Scoping)
                if (!isHighestPermission && identity.IsManager)
                {
                    // Manager: Chỉ thấy nhân viên phòng mình
                    query = query.Where(e => e.DepartmentId == identity.DepartmentId);
                }
                else if (isHighestPermission && deptId.HasValue)
                {
                    // Admin/Director: Lọc theo phòng ban nếu chọn trên UI
                    query = query.Where(e => e.DepartmentId == deptId.Value);
                }

                // 3. Filter theo Trạng thái
                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    query = query.Where(e => e.Status == status);
                }

                // 4. Search logic
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(e => e.FullName.Contains(search));
                }

                // 5. Sorting theo JoinDate
                if (joinDateDes.HasValue)
                {
                    query = joinDateDes.Value
                        ? query.OrderByDescending(e => e.JoinDate)
                        : query.OrderBy(e => e.JoinDate);
                }

                // 6. Thực thi Query
                var employees = await query.ToListAsync();

                // 7. Mapping & Data Masking (Sử dụng isHighestPermission)
                var dtoList = employees.Select(e => new EmployeeListDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    DoB = e.DoB,
                    PositionName = e.Position?.Name ?? "N/A",
                    Status = e.Status,
                    JoinDate = e.JoinDate,
                    HasAccount = e.User != null,
                    DepartmentName = e.Department?.Name ?? "N/A",

                    // --- LOGIC MASKING CHO MANAGER ---
                    // Chỉ Sếp tổng mới thấy tên Username và Lương
                    Username = isHighestPermission ? e.User?.Username : "Hidden",
                    BaseSalary = isHighestPermission ? e.ActiveContract?.BasicSalary : null,

                    // Số điện thoại cũng nên ẩn với Manager nếu không phải lính trực tiếp? 
                    // Ở đây mình để isHighestPermission check cho đồng bộ
                    Phone = isHighestPermission ? e.Phone : "********"
                }).AsEnumerable();

                return ResponseDto<IEnumerable<EmployeeListDto>>.SuccessResult(dtoList);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<EmployeeListDto>>.FailResult("500", "Lỗi lấy danh sách: " + ex.Message);
            }
        }

        public async Task<ResponseDto<int>> CreateEmployeeAsync(UserIdentity identity, CreateEmployeeDto dto)
        {
            // 1. BR: Chỉ Admin hoặc Director mới có quyền Add
            if (!identity.IsAdmin && !identity.IsDirector)
            {
                return ResponseDto<int>.FailResult("403", "Bạn không có quyền thực hiện chức năng này.");
            }

            try
            {
                // 2. Check trùng Email (Email cá nhân hoặc Email công việc nếu có)
                var isEmailExist = await _unitOfWork.Employees.GetEmployeesQuery()
                    .AnyAsync(e => e.Email.ToLower() == dto.Email.ToLower() && !e.IsDeleted);

                if (isEmailExist)
                {
                    return ResponseDto<int>.FailResult("EMP_001", "Email này đã tồn tại trong hệ thống.");
                }

                // 3. Khởi tạo Entity Nhân viên mới
                var newEmployee = new Employee
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    DoB = dto.DoB,
                    DepartmentId = dto.DepartmentId,
                    PositionId = dto.PositionId,
                    JoinDate = dto.JoinDate,
                    Status = "Active", // Mặc định là đang hoạt động
                    CreatedDate = DateTime.Now.Date,
                    IsDeleted = false
                };

                // 4. Lưu vào DB qua Unit of Work
                 _unitOfWork.Employees.Add(newEmployee);
                var result = await _unitOfWork.CommitAsync();

                if (result > 0)
                {
                    // Trả về ID của nhân viên vừa tạo để FE có thể mở tiếp Popup tạo Account (C2)
                    return ResponseDto<int>.SuccessResult(newEmployee.Id);
                }

                return ResponseDto<int>.FailResult("500", "Lưu dữ liệu thất bại.");
            }
            catch (Exception ex)
            {
                return ResponseDto<int>.FailResult("500", "Lỗi hệ thống: " + ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> UpdateEmployeeAsync(UserIdentity identity, int id, UpdateEmployeeDto dto)
        {
            var isHighest = identity.IsAdmin || identity.IsDirector;

            try
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (employee == null || employee.IsDeleted)
                    return ResponseDto<bool>.FailResult("404", "Không tìm thấy nhân viên.");

                // BR: Manager chỉ được sửa người trong phòng mình
                if (!isHighest && identity.IsManager && employee.DepartmentId != identity.DepartmentId)
                    return ResponseDto<bool>.FailResult("403", "Bạn không có quyền sửa nhân viên phòng khác.");

                // Cập nhật thông tin
                employee.FullName = dto.FullName;
                employee.Phone = dto.Phone;
                employee.DoB = dto.DoB;
                employee.DepartmentId = dto.DepartmentId;
                employee.PositionId = dto.PositionId;
                employee.Status = dto.Status;

                await _unitOfWork.CommitAsync();
                return ResponseDto<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailResult("500", ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> SoftDeleteEmployeeAsync(UserIdentity identity, int id)
        {
            if (!identity.IsAdmin && !identity.IsDirector)
                return ResponseDto<bool>.FailResult("403", "Chỉ Admin/Director mới có quyền xóa.");

            try
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);
                if (employee == null) return ResponseDto<bool>.FailResult("404", "Không tìm thấy.");

                employee.IsDeleted = true;
                _unitOfWork.Users.Delete(employee.User!);                          

                await _unitOfWork.CommitAsync();
                return ResponseDto<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailResult("500", ex.Message);
            }
        }

        public async Task<ResponseDto<EmployeeListDto>> GetEmployeeDetailAsync(UserIdentity identity, int id)
        {
            try
            {
                // 1. Lấy dữ liệu thô từ Repo (đã bao gồm các bảng liên quan)
                var employee = await _unitOfWork.Employees.GetByIdAsync(id);

                if (employee == null || employee.IsDeleted)
                    return ResponseDto<EmployeeListDto>.FailResult("404", "Không tìm thấy nhân viên.");

                // 2. Logic Phân quyền xem chi tiết (Quan trọng!)
                var isHighest = identity.IsAdmin || identity.IsDirector;
                var isOwnProfile = identity.EmployeeId == id;
                var isManagerOfDept = identity.IsManager && employee.DepartmentId == identity.DepartmentId;

                // Nếu không phải Sếp tổng, không phải chính chủ, và không phải Manager của phòng đó -> Chặn
                if (!isHighest && !isOwnProfile && !isManagerOfDept)
                {
                    return ResponseDto<EmployeeListDto>.FailResult("403", "Bạn không có quyền xem thông tin này.");
                }

                // 3. Mapping sang DTO kèm Masking dữ liệu
                var dto = new EmployeeListDto
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    Email = employee.Email,
                    Phone = (isHighest || isOwnProfile) ? employee.Phone : "********", // Chính chủ hoặc Sếp mới thấy Phone
                    DoB = employee.DoB,
                    PositionName = employee.Position?.Name ?? "N/A",
                    DepartmentName = employee.Department?.Name ?? "N/A",
                    Status = employee.Status,
                    JoinDate = employee.JoinDate,
                    HasAccount = employee.User != null,
                    Username = employee.User?.Username,
                    Address = (isHighest || isOwnProfile) ? employee.Phone : "N/A",

                    // Lương là thông tin siêu nhạy cảm: Chỉ Sếp tổng hoặc Chính chủ mới thấy
                    BaseSalary = (isHighest || isOwnProfile) ? employee.ActiveContract?.BasicSalary : null
                };

                return ResponseDto<EmployeeListDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ResponseDto<EmployeeListDto>.FailResult("500", "Lỗi: " + ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> PromoteToManagerAsync(UserIdentity identity, int employeeId)
        {
            // 1. Check quyền: Chỉ Admin/Director mới được phong sếp
            if (!identity.IsAdmin && !identity.IsDirector)
                return ResponseDto<bool>.FailResult("403", "Bạn không có quyền bổ nhiệm.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
                if (employee == null || employee.IsDeleted)
                    return ResponseDto<bool>.FailResult("404", "Không tìm thấy nhân viên.");

                // 2. Cập nhật ManagerId cho Phòng ban của nhân viên này
                var dept = await _unitOfWork.Departments.GetByIdAsync(employee.DepartmentId);
                if (dept == null) return ResponseDto<bool>.FailResult("404", "Phòng ban không tồn tại.");

                dept.ManagerId = employeeId;

                var user = await _unitOfWork.Users.GetByEmployeeIdAsync(employeeId);
                if (user != null)
                {
                    user.UserRoles.Add(new Data.Entities.Identity.UserRole { RoleId = 3, UserId = user.Id });
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
