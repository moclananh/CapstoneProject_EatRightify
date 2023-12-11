using Component.Application.Utilities.Promotions;
using Component.Data.Entities;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        public PromotionsController(
            IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByPromotionCode(Guid code)
        {

            var promotions = await _promotionService.GetByPromotionCode(code);
            var timeNow = DateTime.Now;
            if (timeNow <= promotions.FromDate || timeNow >= promotions.ToDate)
            {
                return BadRequest("The gift code is out of time.");
            }

            if (promotions == null)
            {
                return BadRequest();
            }
            return Ok(promotions);
        }
    }
}
