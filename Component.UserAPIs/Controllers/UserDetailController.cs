using Component.Application.System.Users.UserDetail;
using Component.Application.Utilities.Blogs;
using Component.ViewModels.System.Users;
using Component.ViewModels.Utilities.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserDetailController : Controller
    {
        private readonly IUserDetailService _userDetailService;

        public UserDetailController(
            IUserDetailService userDetailService)
        {
            _userDetailService = userDetailService;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserDetailVm request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var check = await _userDetailService.GetById(request.Id);
            if (check!=null)
            {
                return BadRequest();
            }
            var result = await _userDetailService.Create(request);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }



        [HttpPut("{userId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] Guid userId, [FromForm] UserDetailVm request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = userId;
            var checkUserExist = await _userDetailService.GetById(userId);
            if (checkUserExist == null)
            {
                return BadRequest();
            }
            var affectedResult = await _userDetailService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = await _userDetailService.GetById(id);
            if (userId == null)
            {
                return BadRequest();
            }
            return Ok(userId);
        }

    }
}
