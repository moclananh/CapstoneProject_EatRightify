using Component.Application.Utilities.Comments;
using Component.ViewModels.Utilities.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagerPolicy")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(
            ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll(int productId)
        {
            var item = await _commentService.GetAll(productId);
            return Ok(item);
        }


        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetCommentPagingRequest request)
        {
            var comments = await _commentService.GetAllCommentByProductIdPaging(request);
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var comments = await _commentService.GetById(id);
            if (comments == null)
            {
                return BadRequest();
            }
            return Ok(comments);
        }


        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete(int commentId)
        {
            var affectedResult = await _commentService.Delete(commentId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}
