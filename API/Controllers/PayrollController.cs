using System.Threading.Tasks;
using HRFlow.Business.Interfaces;
using HRFlow.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Director")]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePayroll([FromQuery] int month, [FromQuery] int year)
        {
            var response = await _payrollService.CalculateMonthlyPayrollAsync(month, year);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("preview")]
        public async Task<IActionResult> GetPreview([FromQuery] int month, [FromQuery] int year)
        {
            var response = await _payrollService.GetPayrollPreviewAsync(month, year);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}