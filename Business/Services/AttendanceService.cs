using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Dtos.AttendanceDtos;
using Data.Abstractions;
using Data.Entities.Attendance;
using Data.Entities.HR;
using HRFlow.Business.Interfaces;
using Shared.Wrappers;

namespace HRFlow.Business.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto<List<TimeLogDto>>> GetMyCalendarAsync(int employeeId, int month, int year)
        {
            var logs = await _unitOfWork.TimeLogs.GetLogsByMonthAsync(employeeId, month, year);

            var data = logs.Select(l => new TimeLogDto
            {
                Id = l.Id,
                Date = l.Date,
                CheckIn = l.CheckIn.ToString(@"hh\:mm"),
                CheckOut = l.CheckOut?.ToString(@"hh\:mm"),
                Status = l.Status
            }).ToList();

            return ResponseDto<List<TimeLogDto>>.SuccessResult(data);
        }

        public async Task<ResponseDto<TimeLogDto>> CheckInAsync(int employeeId)
        {
            var today = DateTime.Today;
            var currentTime = DateTime.Now.TimeOfDay;

            var existingLog = await _unitOfWork.TimeLogs.GetLogByDateAsync(employeeId, today);
            if (existingLog != null)
            {
                return ResponseDto<TimeLogDto>.FailResult("Bạn đã Check-in hôm nay rồi.");
            }

            var newLog = new TimeLog
            {
                EmployeeId = employeeId,
                Date = today,
                CheckIn = currentTime,
                Status = currentTime <= new TimeSpan(8, 30, 0) ? "Đúng giờ" : "Đi muộn"
            };

            await _unitOfWork.TimeLogs.AddAsync(newLog);
            await _unitOfWork.CommitAsync();

            return ResponseDto<TimeLogDto>.SuccessResult(new TimeLogDto
            {
                Id = newLog.Id,
                Date = newLog.Date,
                CheckIn = newLog.CheckIn.ToString(@"hh\:mm"),
                Status = newLog.Status
            });
        }

        public async Task<ResponseDto<TimeLogDto>> CheckOutAsync(int employeeId)
        {
            var today = DateTime.Today;
            var currentTime = DateTime.Now.TimeOfDay;

            var existingLog = await _unitOfWork.TimeLogs.GetLogByDateAsync(employeeId, today);
            if (existingLog == null)
            {
                return ResponseDto<TimeLogDto>.FailResult("Bạn chưa Check-in nên không thể Check-out.");
            }

            if (existingLog.CheckOut != null)
            {
                return ResponseDto<TimeLogDto>.FailResult("Bạn đã Check-out trước đó rồi.");
            }

            existingLog.CheckOut = currentTime;
            _unitOfWork.TimeLogs.Update(existingLog);
            await _unitOfWork.CommitAsync();

            return ResponseDto<TimeLogDto>.SuccessResult(new TimeLogDto
            {
                Id = existingLog.Id,
                Date = existingLog.Date,
                CheckIn = existingLog.CheckIn.ToString(@"hh\:mm"),
                CheckOut = existingLog.CheckOut?.ToString(@"hh\:mm"),
                Status = existingLog.Status
            });
        }

        public async Task<ResponseDto<bool>> CreateLeaveRequestAsync(int employeeId, CreateLeaveRequestDto dto)
        {
            if (dto.FromDate.Date > dto.ToDate.Date)
            {
                return ResponseDto<bool>.FailResult("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
            }

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveType = dto.LeaveType,
                FromDate = dto.FromDate.Date,
                ToDate = dto.ToDate.Date,
                Reason = dto.Reason,
                Status = "Pending",
                TotalDays = (dto.ToDate.Date - dto.FromDate.Date).Days + 1
            };

            await _unitOfWork.LeaveRequests.AddAsync(leaveRequest);
            var result = await _unitOfWork.CommitAsync();

            return result > 0
                ? ResponseDto<bool>.SuccessResult(true)
                : ResponseDto<bool>.FailResult("DB_ERROR", "Lỗi lưu dữ liệu.");
        }
        public async Task<ResponseDto<List<TimeLogDto>>> GetEmployeeCalendarForDirectorAsync(int employeeId, int month, int year)
        {
            // 1. Kiểm tra xem EmployeeId có tồn tại trong hệ thống không
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return ResponseDto<List<TimeLogDto>>.FailResult("NOT_FOUND", "Không tìm thấy nhân viên với mã này.");
            }

            // 2. Gọi Repository để lấy danh sách chấm công trong tháng/năm chỉ định
            // Hàm GetLogsByMonthAsync bác đã viết ở Repository các bước trước rồi đấy
            var logs = await _unitOfWork.TimeLogs.GetLogsByMonthAsync(employeeId, month, year);

            // 3. Mapping dữ liệu từ Entity sang DTO để trả về cho Frontend
            var data = logs.Select(l => new TimeLogDto
            {
                Id = l.Id,
                Date = l.Date,
                // Ép kiểu TimeSpan sang string định dạng HH:mm để FE dễ hiển thị
                CheckIn = l.CheckIn.ToString(@"hh\:mm"),
                CheckOut = l.CheckOut?.ToString(@"hh\:mm"),
                Status = l.Status
            })
            .OrderBy(l => l.Date) // Sắp xếp theo ngày tăng dần cho chuẩn lịch
            .ToList();

            // 4. Trả về kết quả thành công kèm dữ liệu
            return ResponseDto<List<TimeLogDto>>.SuccessResult(data);
        }
    }
}