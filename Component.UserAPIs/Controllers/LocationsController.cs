using Component.Application.Utilities.Locations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Locations;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationsController(
            ILocationService locationService)
        {
            _locationService = locationService;
        }


        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetLocationPagingRequest request)
        {
            var location = await _locationService.GetAllPaging(request);
            return Ok(location);
        }

        [HttpGet("getAllLocation")]
        public async Task<IActionResult> GetAllLocation()
        {
            var location = await _locationService.GetAll();
            return Ok(location);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var location = await _locationService.GetById(id);
            if (location == null)
            {
                return BadRequest();
            }
            return Ok(location);
        }
    }
    
}
