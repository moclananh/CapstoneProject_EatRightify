using Component.Application.Utilities.Blogs;
using Component.Application.Utilities.Promotions;
using Component.Data.Entities;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagerPolicy")]
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

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll(string? keyword)
        {
            var promotions = await _promotionService.GetAll(keyword);
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
        public async Task<IActionResult> Create([FromBody] PromotionCreateRequest request)
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
        public async Task<IActionResult> Update([FromRoute] int promotionId, [FromBody] PromotionUpdateRequest request)
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
            try
            {
                var affectedResult = await _promotionService.Update(request);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("UpdateStatusOnly")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateStatusOnly([FromBody] UpdateStatusOnlyRequest request)
        {
            var check = await _promotionService.GetById(request.PromotionId);
            if (check == null) return BadRequest();
            await _promotionService.UpdateStatusOnly(request);
            return Ok();

        }

        [HttpDelete("{promotionId}")]
        public async Task<IActionResult> Delete(int promotionId)
        {
            var check = await _promotionService.GetById(promotionId);
            if (check == null) return BadRequest();
            var affectedResult = await _promotionService.Delete(promotionId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}
