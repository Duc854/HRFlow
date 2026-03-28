using Business.Abstractions;
using Business.Dtos.UserDtos.AccountDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrappers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Cấp tài khoản mới cho nhân viên (Quan hệ 1-1)
        /// </summary>
        /// 
        [Authorize(Roles = "Admin,Director")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDto input)
        {
            var result = await _accountService.CreateAccountAsync(input);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Khóa hoặc Mở khóa tài khoản (Active/Inactive)
        /// </summary>
        /// 
        [Authorize(Roles = "Admin,Director")]
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _accountService.ToggleStatusAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Reset mật khẩu nhân viên về một mật khẩu mới
        /// </summary>
        /// 
        [Authorize(Roles = "Admin,Director")]
        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            var result = await _accountService.ResetPasswordAsync(id, newPassword);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("user_list")]
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> GetUserList([FromQuery] string? search, [FromQuery] string? role)
        {
            var result = await _accountService.GetUserListAsync(search, role);
            return Ok(result);
        }

        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> UpdateRoles(int id, [FromBody] List<string> roleNames)
        {
            var result = await _accountService.UpdateUserRolesAsync(id, roleNames);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "404" => NotFound(result),
                    "400" => BadRequest(result),
                    _ => StatusCode(500, result)
                };
            }

            return Ok(result);
        }
    }
}
