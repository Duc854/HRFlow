using Business.Abstractions;
using Business.Dtos.DashboardDto;
using Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<ResponseDto<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                var stats = new DashboardStatsDto();

                // 1. Tổng số nhân viên & Phòng ban (Chỉ tính những người chưa bị xóa)
                stats.TotalEmployees = await _unitOfWork.Employees.GetEmployeesQuery()
                    .CountAsync(e => !e.IsDeleted);

                stats.TotalDepartments = await _unitOfWork.Departments.GetDepartmentsQuery()
                    .CountAsync(d => !d.IsDeleted);

                // 2. Nhân viên mới trong tháng
                stats.NewEmployeesThisMonth = await _unitOfWork.Employees.GetEmployeesQuery()
                    .CountAsync(e => e.JoinDate >= firstDayOfMonth && !e.IsDeleted);

                // 3. Dữ liệu biểu đồ: Group theo Department
                stats.EmployeeByDepartment = await _unitOfWork.Employees.GetEmployeesQuery()
                    .Where(e => !e.IsDeleted && e.Department != null)
                    .GroupBy(e => e.Department.Name)
                    .Select(g => new ChartDataDto
                    {
                        Label = g.Key,
                        Value = g.Count()
                    }).ToListAsync();

                // 4. Cảnh báo hợp đồng sắp hết hạn (Ví dụ trong 30 ngày tới)
                var deadline = today.AddDays(30);
                stats.ExpiringContracts = await _unitOfWork.Employees.GetEmployeesQuery()
                    .Where(e => !e.IsDeleted
                                && e.ActiveContract != null
                                && e.ActiveContract.EndDate.HasValue // Đảm bảo EndDate có giá trị
                                && e.ActiveContract.EndDate >= today
                                && e.ActiveContract.EndDate <= deadline)
                    .OrderBy(e => e.ActiveContract.EndDate)
                    .Select(e => new ExpiringContractDto
                    {
                        EmployeeId = e.Id,
                        FullName = e.FullName,

                        // 1. Ép kiểu DateTime? sang DateTime bằng .Value
                        ExpiryDate = e.ActiveContract.EndDate.Value,

                        // 2. Ép kiểu TimeSpan? sang TimeSpan để lấy .Days
                        DaysLeft = (e.ActiveContract.EndDate.Value - today).Days
                    })
                    .Take(10)
                    .ToListAsync();

                return ResponseDto<DashboardStatsDto>.SuccessResult(stats);
            }
            catch (Exception ex)
            {
                return ResponseDto<DashboardStatsDto>.FailResult("500", ex.Message);
            }
        }
    }
}
