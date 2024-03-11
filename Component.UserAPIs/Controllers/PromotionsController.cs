using Component.Application.Utilities.Promotions;
using Component.Data.Entities;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetByPromotionCode(string code)
        {
            var promotions = await _promotionService.GetByPromotionCode(code);
            return Ok(promotions);
        }
    }
}
