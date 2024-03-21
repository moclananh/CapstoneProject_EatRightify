using Component.Application.System.Users;
using Component.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Component.AdminAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Authencate(request);

            if (string.IsNullOrEmpty(result.ResultObj))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Register(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Create(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //PUT: http://localhost/api/users/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Update(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}/roles")]
        public async Task<IActionResult> RoleAssign(Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RoleAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //http://localhost/api/users/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request)
        {
            var products = await _userService.GetUsersPaging(request);
            return Ok(products);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword)
        {
            var item = await _userService.GetAll(keyword);
            return Ok(item);
        }

        [HttpGet("GetTotalUser")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTotalUser([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var item = await _userService.TotalUser(startDate, endDate);
            return Ok(item);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.Delete(id);
            return Ok(result);
        }

        [HttpPut("BanAccount/{UserId}/{status}")]
        public async Task<IActionResult> BanAccount(Guid UserId, bool status)
        {
            var result = await _userService.BanAccount(UserId, status);
            return Ok(result);
        }

        [HttpPost("ForgotPassword/{Email}")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ForgotPassword(Email);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ResetPassword(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("GetVerifyCode/{Email}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVerifyCode(string Email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.GetVerifyCode(Email);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("VerifyAccount")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.VerifyAccount(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(string token)
        {
            var response = await _userService.RefreshToken(token);
            if (!response.IsSuccessed)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
