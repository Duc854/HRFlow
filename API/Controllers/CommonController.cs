using Business.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _commonService;
        public CommonController(ICommonService commonService) => _commonService = commonService;

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var result = await _commonService.GetDepartmentsAsync();
            return Ok(result);
        }

        [HttpGet("positions")]
        public async Task<IActionResult> GetPositions()
        {
            var result = await _commonService.GetPositionsAsync();
            return Ok(result);
        }
    }
}
