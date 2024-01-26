using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Locations
{
    public interface ILocationService
    {
        Task<List<LocationVm>> GetAll();
        Task<PagedResult<LocationVm>> GetAllPaging(GetLocationPagingRequest request);

        Task<LocationVm> GetById(int id);
        Task<Location> Create(LocationCreateRequest request);

        Task<int> Update(LocationUpdateRequest request);

        Task<int> Delete(int locationId);
    }
}
