using Component.Application.Utilities.Blogs;
using Component.ViewModels.Utilities.Blogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
