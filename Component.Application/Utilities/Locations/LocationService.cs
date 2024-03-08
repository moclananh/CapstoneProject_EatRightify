using Component.Application.System.Users;
using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Locations;
using Microsoft.EntityFrameworkCore;

namespace Component.Application.Utilities.Locations
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;
        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }

     
        public async Task<Location> Create(LocationCreateRequest request)
        {
            var locations = new Location()
            {
                LocationName = request.LocationName,
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                Description = request.Description,
                Status = Data.Enums.Status.Active,
                DateCreated = DateTime.Now,
                CreatedBy = request.CreatedBy,

            };

            _context.Locations.Add(locations);
            await _context.SaveChangesAsync();
            return locations;
        }

        public async Task<int> Delete(int locationId)
        {
            var check = await GetById(locationId);
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.LocationId == locationId);
            if (location == null)
            {
                throw new EShopException($"Cannot find a location: {locationId}");
            }
            if (check.LocationId != location.LocationId)
            {
                throw new EShopException($"Error to find location: {locationId}");
            }
            _context.Locations.Remove(location);
            return await _context.SaveChangesAsync();
        }


        public async Task<List<LocationVm>> GetAll()
        {
            var query = from l in _context.Locations
                        join u in _context.AppUsers on l.CreatedBy equals u.Id into bu
                        from u in bu.DefaultIfEmpty()
                        select new { l, u };
            var result = await query.Select(x => new LocationVm()
            {
                LocationId = x.l.LocationId,
                LocationName = x.l.LocationName,
                Longitude = x.l.Longitude,
                Latitude = x.l.Latitude,
                Description = x.l.Description,
                Status = x.l.Status,
                DateCreated = x.l.DateCreated,
                CreatedBy = x.u.UserName
            }).ToListAsync();
            result = result.OrderByDescending(x => x.DateCreated).ToList();
            return result;
        }

        public async Task<PagedResult<LocationVm>> GetAllPaging(GetLocationPagingRequest request)
        {
            //1. Select join
            var query = from l in _context.Locations
                        join u in _context.AppUsers on l.CreatedBy equals u.Id into bu
                        from u in bu.DefaultIfEmpty()
                        select new LocationVm()
                        {
                            LocationId = l.LocationId,
                            LocationName = l.LocationName,
                            Longitude = l.Longitude,
                            Latitude = l.Latitude,
                            Description = l.Description,
                            Status = l.Status,
                            DateCreated = l.DateCreated,
                            CreatedBy = u.UserName
                        };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.LocationName.Contains(request.Keyword));


            int totalRow = query.Count();

            var data = query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<LocationVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<LocationVm> GetById(int id)
        {
            var query = from l in _context.Locations
                        join u in _context.AppUsers on l.CreatedBy equals u.Id into bu
                        from u in bu.DefaultIfEmpty()
                        where l.LocationId == id
                        select new { l, u };
            return await query.Select(x => new LocationVm()
            {
                LocationId = x.l.LocationId,
                LocationName = x.l.LocationName,
                Longitude = x.l.Longitude,
                Latitude = x.l.Latitude,
                Description = x.l.Description,
                Status = x.l.Status,
                DateCreated = x.l.DateCreated,
                CreatedBy = x.u.UserName
            }).FirstOrDefaultAsync();

        }

        public async Task<int> Update(LocationUpdateRequest request)
        {
            var location = await _context.Locations.FindAsync(request.LocationId);

            if (location == null) throw new EShopException($"Cannot find a locations with id: {request.LocationId}");
            location.LocationName = request.LocationName;
            location.Longitude = request.Longitude;
            location.Latitude = request.Latitude;
            location.Description = request.Description;
            location.Status = request.Status;

            return await _context.SaveChangesAsync();
        }
    }
}
