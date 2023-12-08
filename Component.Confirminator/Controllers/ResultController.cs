using Component.Application.AI;
using Component.ViewModels.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ConfirminatorAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultController : ControllerBase
    {
        private readonly IAIService _aiService;
        public ResultController(IAIService aiService)
        {
            _aiService = aiService;
        }
        [HttpPut("Update/{resultId}")]
        public async Task<IActionResult> Update(int resultId, UpdateResultRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _aiService.Update(resultId, request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPut("UpdateStatus/{resultId}")]
        public async Task<IActionResult> UpdateStatus(int resultId, UpdateStatusResult request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _aiService.UpdateStatus(resultId, request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
