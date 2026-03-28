using System.Security.Claims;
using System.Threading.Tasks;
using Business.Dtos.AttendanceDtos;
using HRFlow.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        private int GetCurrentEmployeeId()
        {
            var empIdStr = User.FindFirstValue("EmployeeId");
            return int.TryParse(empIdStr, out var empId) ? empId : 0;
        }

        [HttpGet("my-calendar")]
        public async Task<IActionResult> GetMyCalendar([FromQuery] int month, [FromQuery] int year)
        {
            var empId = GetCurrentEmployeeId();
            if (empId == 0) return Unauthorized();

            var response = await _attendanceService.GetMyCalendarAsync(empId, month, year);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn()
        {
            var empId = GetCurrentEmployeeId();
            if (empId == 0) return Unauthorized();

            var response = await _attendanceService.CheckInAsync(empId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            var empId = GetCurrentEmployeeId();
            if (empId == 0) return Unauthorized();

            var response = await _attendanceService.CheckOutAsync(empId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("leave-request")]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDto dto)
        {
            var empId = GetCurrentEmployeeId();
            if (empId == 0) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _attendanceService.CreateLeaveRequestAsync(empId, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}