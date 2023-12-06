using Component.Application.Statistical;
using Component.ViewModels.Statistical;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalController : ControllerBase
    {
        private readonly IStatisticalService _statisticalService;

        public StatisticalController(
            IStatisticalService statiticalService)
        {
            _statisticalService = statiticalService;
        }

        [HttpGet]
        public async Task<IActionResult> Statistical([FromQuery] StatisticalRequest request)
        {
            var statistical = await _statisticalService.Statistical(request);
            return Ok(statistical);
        }
    }
}
