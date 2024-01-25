using Component.Application.AI;
using Component.ViewModels.AI;
using Component.ViewModels.Utilities.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultsController : ControllerBase
    {

        private readonly IAIService _aiService;
        public ResultsController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet("GetResultByUserId/{userId}")]
        public async Task<IActionResult> GetResultByUserId(Guid userId)
        {
            var result = await _aiService.GetByUserId(userId);

            if (result == null)
            {
                return BadRequest(); // Return 404 Not Found if the order with the given ID is not found.
            }

            return Ok(result); // Return the order with a 200 OK status code.
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateResultRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _aiService.Create(request);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }     
    }
}
