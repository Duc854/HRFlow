using Business.Abstractions;
using Business.Dtos.AuthDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Wrappers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto input)
        {
            var result = await _authService.Login(input);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto input)
        {
            if (string.IsNullOrEmpty(input.RefreshToken))
            {
                return BadRequest(ResponseDto<LoginResponseDto>.FailResult("EmptyToken", "Refresh Token không được để trống"));
            }

            var result = await _authService.RefreshToken(input.RefreshToken);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
