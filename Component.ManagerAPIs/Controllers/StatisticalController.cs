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

        [HttpGet("GetTotalQuantity")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTotalQuantity([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var item = await _statisticalService.TotalQuantityOfOrder(startDate, endDate);
            return Ok(item);
        }

        [HttpGet("getAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(DateTime? startDate, DateTime? endDate)
        {
            var item = await _statisticalService.GetAll(startDate, endDate);
            return Ok(item);
        }

        [HttpGet("GetListProductInteraction")]
        public async Task<IActionResult> GetListProductInteraction(string? keyword)
        {
            var item = await _statisticalService.GetListProductInteractions(keyword);
            return Ok(item);
        }

        [HttpGet("GetListUserInteraction")]
        public async Task<IActionResult> GetListUserInteraction(string? keyword)
        {
            var item = await _statisticalService.GetListUserInteractions(keyword);
            return Ok(item);
        }

        [HttpGet("GetListCustomerLoyal")]
        public async Task<IActionResult> GetListCustomerLoyal(string? keyword)
        {
            var item = await _statisticalService.GetListCustomerLoyal(keyword);
            return Ok(item);
        }
    }
}
