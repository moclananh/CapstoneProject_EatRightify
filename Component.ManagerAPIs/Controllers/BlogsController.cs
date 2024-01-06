using Component.Application.Catalog.Categories;
using Component.Application.Utilities.Blogs;
using Component.ViewModels.Catalog.Categories;
using Component.ViewModels.Utilities.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagerPolicy")]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(
            IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _blogService.GetAll();
            return Ok(blogs);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetBlogPagingRequest request)
        {
            var blogs = await _blogService.GetAllPaging(request);
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var blogs = await _blogService.GetById(id);
            if (blogs == null)
            {
                return BadRequest();
            }
            return Ok(blogs);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlogCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _blogService.Create(request);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut("{blogId}")]
        public async Task<IActionResult> Update([FromRoute] int blogId, [FromBody] BlogUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = blogId;
            var checkBlogExist = await _blogService.GetById(blogId);
            if (checkBlogExist == null)
            {
                return BadRequest();
            }
            var affectedResult = await _blogService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{blogId}")]
        public async Task<IActionResult> Delete(int blogId)
        {
            var affectedResult = await _blogService.Delete(blogId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}
