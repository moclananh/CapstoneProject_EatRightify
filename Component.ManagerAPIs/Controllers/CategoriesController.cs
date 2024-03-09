using Component.Application.Catalog.Categories;
using Component.ViewModels.Catalog.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*    [Authorize(Policy = "ManagerPolicy")]*/
    [AllowAnonymous]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? keyword)
        {
            var products = await _categoryService.GetAll(keyword);
            return Ok(products);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetCategoryPagingRequest request)
        {
            var categories = await _categoryService.GetAllPaging(request);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetById(id);
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _categoryService.Create(request);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> Update([FromRoute] int categoryId, [FromBody] CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = categoryId;
            try
            {
                var affectedResult = await _categoryService.Update(request);
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            var affectedResult = await _categoryService.Delete(categoryId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}
