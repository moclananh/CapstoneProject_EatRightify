using Component.Data.Entities;
using Component.ViewModels.Utilities.Locations;

namespace Component.Application.Utilities.Locations
{
    public interface ILocationService
    {
        Task<List<LocationVm>> GetAll();
        Task<List<LocationVm>> GetAllLocationActive();
        Task<LocationVm> GetById(int id);
        Task<Location> Create(LocationCreateRequest request);
        Task<int> Update(LocationUpdateRequest request);
        Task<int> Delete(int locationId);
        //Task<PagedResult<LocationVm>> GetAllPaging(GetLocationPagingRequest request);
    }
}
