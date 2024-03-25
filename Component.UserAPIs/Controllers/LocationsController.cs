using Component.Application.Utilities.Locations;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("getAllLocation")]
        public async Task<IActionResult> GetAllLocation()
        {
            var location = await _locationService.GetAllLocationActive();
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

        /*    [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetLocationPagingRequest request)
        {
            var location = await _locationService.GetAllPaging(request);
            return Ok(location);
        }*/
    }
}
