using System.Security.Claims;
using System.Threading.Tasks;
using Business.Dtos.EmployeeDtos;
using HRFlow.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrappers; // Dùng chuẩn ResponseDto mới

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out var userId) ? userId : 0;
        }

        private int GetCurrentEmployeeId()
        {
            var empIdStr = User.FindFirstValue("EmployeeId");
            return int.TryParse(empIdStr, out var empId) ? empId : 0;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var response = await _profileService.GetMyProfileAsync(userId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var response = await _profileService.ChangeMyPasswordAsync(userId, dto);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("my-contracts")]
        public async Task<IActionResult> GetMyContracts()
        {
            var employeeId = GetCurrentEmployeeId();
            if (employeeId == 0)
            {
                // FIX: Dùng FailResult thay vì new ResponseDto
                return BadRequest(ResponseDto<object>.FailResult("Tài khoản này chưa được liên kết với hồ sơ nhân viên nào!"));
            }

            var response = await _profileService.GetMyContractsAsync(employeeId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}