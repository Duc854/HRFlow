using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Dtos.AttendanceDtos;
using Shared.Wrappers;

namespace HRFlow.Business.Interfaces
{
    public interface IAttendanceService
    {
        Task<ResponseDto<List<TimeLogDto>>> GetMyCalendarAsync(int employeeId, int month, int year);
        Task<ResponseDto<TimeLogDto>> CheckInAsync(int employeeId);
        Task<ResponseDto<TimeLogDto>> CheckOutAsync(int employeeId);
        Task<ResponseDto<bool>> CreateLeaveRequestAsync(int employeeId, CreateLeaveRequestDto dto);
        Task<ResponseDto<List<TimeLogDto>>> GetEmployeeCalendarForDirectorAsync(int employeeId, int month, int year);
    }
}