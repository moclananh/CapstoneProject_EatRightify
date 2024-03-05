using Component.Application.AI;
using Component.Data.Enums;
using Component.ViewModels.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ConfirminatorAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "VerifierPolicy")]
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
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _aiService.UpdateStatus(resultId, request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPut("UpdateIsSend/{resultId}")]
        public async Task<IActionResult> UpdateIsSend(int resultId, UpdateIsSendRequest request)
        {
            var check = await _aiService.GetById(resultId);
            if (check == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _aiService.UpdateIsSend(resultId, request);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] ResultPagingRequest request)
        {
            var products = await _aiService.GetAllPaging(request);
            return Ok(products);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll(string? keyword, ResultStatus? status)
        {
            var item = await _aiService.GetAll(keyword, status);
            return Ok(item);
        }

        [HttpGet("GetById/{ResultId}")]
        public async Task<IActionResult> GetResultById(int ResultId)
        {
            var result = await _aiService.GetById(ResultId);

            if (result == null)
            {
                return BadRequest(); // Return 404 Not Found if the order with the given ID is not found.
            }

            return Ok(result); // Return the order with a 200 OK status code.
        }

        [HttpDelete("Delete/{ResultId}")]
        public async Task<IActionResult> Delete(int ResultId)
        {
            var result = await _aiService.Delete(ResultId);
            if (result == null)
            {
                return BadRequest(); // Return 404 Not Found if the order with the given ID is not found.
            }
            return Ok(); // Return the order with a 200 OK status code.
        }

        [HttpPost("GetResultEmail/{Email}")]
        public async Task<IActionResult> GetResultEmail(string Email, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _aiService.GetResultEmail(Email, id);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
