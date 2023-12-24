using Component.Application.Utilities.Blogs;
using Component.Application.Utilities.Promotions;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   /* [Authorize(Policy = "ManagerPolicy")]*/
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(
            IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetPromotionPagingRequest request)
        {
            var promotions = await _promotionService.GetAllPaging(request);
            return Ok(promotions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promotions = await _promotionService.GetById(id);
            if (promotions == null)
            {
                return BadRequest();
            }
            return Ok(promotions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PromotionCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _promotionService.Create(request);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut("{promotionId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] int promotionId, [FromForm] PromotionUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = promotionId;
            var checkPromotionExist = await _promotionService.GetById(promotionId);
            if (checkPromotionExist == null)
            {
                return BadRequest();
            }
            var affectedResult = await _promotionService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{promotionId}")]
        public async Task<IActionResult> Delete(int promotionId)
        {
            var check = _promotionService.GetById(promotionId);
            if (check == null) return BadRequest();
            var affectedResult = await _promotionService.Delete(promotionId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}
