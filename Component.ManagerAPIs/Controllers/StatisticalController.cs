using Component.Application.Statistical;
using Component.ViewModels.Statistical;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagerPolicy")]
    public class StatisticalController : ControllerBase
    {
        private readonly IStatisticalService _statisticalService;

        public StatisticalController(
            IStatisticalService statiticalService)
        {
            _statisticalService = statiticalService;
        }

        [HttpGet]
        public async Task<IActionResult> Statistical([FromQuery] StatisticalPagingRequest request)
        {
            var statistical = await _statisticalService.Statistical(request);
            return Ok(statistical);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] StatisticalRequest request)
        {
            var item = await _statisticalService.GetAll(request);
            return Ok(item);
        }
    }
}
