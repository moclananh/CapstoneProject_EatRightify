
using Component.Application.Utilities.Comments;
using Component.ViewModels.Utilities.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(
            ICommentService commentService)
        {
            _commentService = commentService;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _commentService.Create(request);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut("{userId}/{commentId}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] Guid userId, int commentId, [FromBody] CommentUpdateRequest request)
        {
            var userComments = await _commentService.GetById(commentId);
            var check = userComments.UserId.Equals(userId);
            if (check != true)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = commentId;
            var checkBlogExist = await _commentService.GetById(commentId);
            if (checkBlogExist == null)
            {
                return BadRequest();
            }
            var affectedResult = await _commentService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{userId}/{commentId}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid userId, int commentId)
        {
            var userComments = await _commentService.GetById(commentId);
            var check = userComments.UserId.Equals(userId);
            if (check != true)
            {
                return BadRequest();
            }

            var affectedResult = await _commentService.Delete(commentId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}

