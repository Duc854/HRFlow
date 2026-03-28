using System.Security.Claims;
using System.Threading.Tasks;
using Data.Entities.Payroll;
using HRFlow.Business.Interfaces;
using HRFlow.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrappers;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        private int GetCurrentEmployeeId()
        {
            var empIdStr = User.FindFirstValue("EmployeeId");
            return int.TryParse(empIdStr, out var empId) ? empId : 0;
        }

        [HttpPost("calculate")]
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> CalculatePayroll([FromQuery] int month, [FromQuery] int year)
        {
            var response = await _payrollService.CalculateMonthlyPayrollAsync(month, year);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("preview")]
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> GetPreview([FromQuery] int month, [FromQuery] int year)
        {
            var response = await _payrollService.GetPayrollPreviewAsync(month, year);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("my-payslip")]
        [Authorize]
        public async Task<IActionResult> GetMyPayslip([FromQuery] int month, [FromQuery] int year)
        {
            var employeeId = GetCurrentEmployeeId();
            var response = await _payrollService.GetEmployeePayslipAsync(employeeId, month, year);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("confirm-payroll")]
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> ConfirmPayroll([FromQuery] int month, [FromQuery] int year)
        {
            var response = await _payrollService.ConfirmPayrollAsync(month, year);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}