using Component.ViewModels.Common;
using Component.ViewModels.Statistical;

namespace Component.Application.Statistical
{
    public interface IStatisticalService
    {
        Task<PagedResult<StatisticalVm>> Statistical(StatisticalPagingRequest request);
        Task<List<StatisticalVm>> GetAll(DateTime? startDate, DateTime? endDate);
        Task<List<ProductInteractionRequest>> GetListProductInteractions(string keyword);
        Task<List<UserInteractionRequest>> GetListUserInteractions(string keyword);
        Task<List<CustomerLoyalRequest>> GetListCustomerLoyal(string keyword);
        Task<decimal> TotalQuantityOfOrder(DateTime? startDate, DateTime? endDate);
    }
}
