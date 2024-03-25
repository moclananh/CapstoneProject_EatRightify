using Component.Application.Utilities.Locations;
using Component.ViewModels.Utilities.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagerPolicy")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _LocationService;

        public LocationsController(
            ILocationService LocationService)
        {
            _LocationService = LocationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Locations = await _LocationService.GetAll();
            return Ok(Locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Locations = await _LocationService.GetById(id);
            if (Locations == null)
            {
                return BadRequest();
            }
            return Ok(Locations);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LocationCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _LocationService.Create(request);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut("{locationId}")]
        public async Task<IActionResult> Update([FromRoute] int locationId, [FromBody] LocationUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.LocationId = locationId;
            var checkLocationExist = await _LocationService.GetById(locationId);
            if (checkLocationExist == null)
            {
                return BadRequest();
            }
            var affectedResult = await _LocationService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{locationId}")]
        public async Task<IActionResult> Delete(int locationId)
        {
            var check = await _LocationService.GetById(locationId);
            if (check == null)
            {
                return BadRequest();
            }
            try
            {
                var affectedResult = await _LocationService.Delete(locationId);
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest();
            } 
        }

        /*
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetLocationPagingRequest request)
        {
            var Locations = await _LocationService.GetAllPaging(request);
            return Ok(Locations);
        }*/
    }
}
