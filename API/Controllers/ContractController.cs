using API.Extensions;
using Business.Abstractions;
using Business.Dtos.ContractDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        /// <summary>
        /// Xem chi tiết một hợp đồng cụ thể
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _contractService.GetContractDetailAsync(identity, id);

            if (!result.Success)
            {
                if (result.ErrorCode == "403") return Forbid();
                if (result.ErrorCode == "404") return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Search hợp đồng với nhiều tiêu chí (Chỉ Admin/Director)
        /// </summary>
        [Authorize(Roles = "Admin,Director")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchContracts([FromQuery] ContractFilterDto filter)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _contractService.SearchContractsAsync(identity, filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tạo hợp đồng mới (Tự động đóng hợp đồng cũ nếu có)
        /// </summary>
        [Authorize(Roles = "Admin,Director")]
        [HttpPost("add_contract")]
        public async Task<IActionResult> AddContract([FromBody] CreateContractDto dto)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _contractService.CreateContractAsync(identity, dto);

            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "403" => Forbid(),
                    "404" => NotFound(result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }


        /// <summary>
        /// Cập nhật thông tin hợp đồng (Chặn sau ngày 15 nếu sửa lương và hợp đồng cũ ko dc sửa)
        /// </summary>
        [Authorize(Roles = "Admin,Director")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContractDto dto)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            var result = await _contractService.UpdateContractAsync(identity, id, dto);

            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "403" => Forbid(),
                    "404" => NotFound(result),
                    "400" => BadRequest(result),
                    _ => StatusCode(500, result)
                };
            }

            return Ok(result);
        }

        /// <summary>
        /// Chấm dứt hợp đồng (Terminate)
        /// Chỉ cho phép chấm dứt từ ngày hôm nay trở đi
        /// </summary>
        /// <param name="id">ID của hợp đồng</param>
        /// <param name="terminateDate">Ngày kết thúc (Để trống mặc định là hôm nay)</param>
        [Authorize(Roles = "Admin,Director")]
        [HttpDelete("{id}/terminate")]
        public async Task<IActionResult> Terminate(int id, [FromQuery] DateTime? terminateDate)
        {
            var identity = HttpContext.GetUserIdentity();
            if (identity == null) return Unauthorized();

            // Gọi Service xử lý logic "Cấm xuyên không" và "Gán null ActiveContract"
            var result = await _contractService.TerminateContractAsync(identity, id, terminateDate);

            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "403" => Forbid(),
                    "404" => NotFound(result),
                    "400" => BadRequest(result), // Trả về lỗi nếu chọn ngày trong quá khứ
                    _ => StatusCode(500, result)
                };
            }

            return Ok(result);
        }
    }
}
