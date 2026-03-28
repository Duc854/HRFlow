using Business.Abstractions;
using Business.Dtos.UserDtos.AccountDtos;
using HRFlow.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrappers;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Director")]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        public ApprovalController(IApprovalService approvalService) => _approvalService = approvalService;

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim?.Value, out var id) ? id : 0;
        }

        [HttpGet("pending-requests")]
        public async Task<IActionResult> GetPending() => Ok(await _approvalService.GetPendingRequestsAsync());

        [HttpGet("on-leave-today")]
        public async Task<IActionResult> GetOnLeaveToday() => Ok(await _approvalService.GetEmployeesOnLeaveTodayAsync());

        [HttpPost("process-request")]
        public async Task<IActionResult> ProcessRequest([FromBody] ApprovalActionDto dto)
        {
            var directorId = GetCurrentUserId();
            var response = await _approvalService.ApproveOrRejectRequestAsync(directorId, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}