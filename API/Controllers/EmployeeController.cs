using API.Extensions;
using Business.Abstractions;
using Business.Dtos.EmployeeDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Roles = "Admin,Director,Manager")]
        [HttpGet("employee_list")]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string? search,
            [FromQuery] int? deptId,
            [FromQuery] string status = "All",
            [FromQuery] bool? joinDateDes = null,
            [FromQuery] bool isDeleted = false)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null)
                return Unauthorized();

            var result = await _employeeService.GetEmployeeListAsync(
                identity,
                search,
                deptId,
                status,
                joinDateDes,
                isDeleted);

            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Director")]
        [HttpPost("add_employee")]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto dto)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _employeeService.CreateEmployeeAsync(identity, dto);

            if (!result.Success)
            {
                // Nếu lỗi do quyền hạn hoặc dữ liệu đầu vào
                return result.ErrorCode == "403" ? Forbid() : BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Director,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _employeeService.UpdateEmployeeAsync(identity, id, dto);
            return result.Success ? Ok(result) : (result.ErrorCode == "403" ? Forbid() : BadRequest(result));
        }

        [Authorize(Roles = "Admin,Director")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _employeeService.SoftDeleteEmployeeAsync(identity, id);
            return result.Success ? Ok(result) : (result.ErrorCode == "403" ? Forbid() : BadRequest(result));
        }

        [Authorize(Roles = "Admin,Director")]
        [HttpPut("{id}/promote-to-manager")]
        public async Task<IActionResult> PromoteToManager(int id)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _employeeService.PromoteToManagerAsync(identity, id);
            return result.Success ? Ok(result) : (result.ErrorCode == "403" ? Forbid() : BadRequest(result));
        }

        [Authorize] // Tất cả ai có Token đều vào được
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeDetail(int id)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _employeeService.GetEmployeeDetailAsync(identity, id);

            if (!result.Success)
            {
                if (result.ErrorCode == "403") return Forbid();
                if (result.ErrorCode == "404") return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
