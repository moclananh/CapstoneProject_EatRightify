using Component.Application.Utilities.Blogs;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("GetAllBlogActive")]
        public async Task<IActionResult> GetAllBlogActive()
        {
            var blogs = await _blogService.GetAllBlogActive();
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

        [HttpPut("AddViewcount")]
        [AllowAnonymous]
        public async Task<IActionResult> AddViewcount(int blogId)
        {
            try
            {
                await _blogService.AddViewcount(blogId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
                /* [HttpGet("paging")]
         public async Task<IActionResult> GetAllPaging([FromQuery] GetBlogPagingRequest request)
         {
             var blogs = await _blogService.GetAllPaging(request);
             return Ok(blogs);
         }*/
    }
}
