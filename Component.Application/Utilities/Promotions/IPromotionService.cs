using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Promotions;

namespace Component.Application.Utilities.Promotions
{
    public interface IPromotionService
    {
        Task<List<PromotionVm>> GetAll(string keyword);
        Task<PagedResult<PromotionVm>> GetAllPaging(GetPromotionPagingRequest request);
        Task<PromotionVm> GetById(int id);
        Task<ApiResult<PromotionVm>> GetByPromotionCode(string code);
        Task<Promotion> Create(PromotionCreateRequest request);
        Task<int> Update(PromotionUpdateRequest request);
        Task<int> Delete(int promotionId);
    }
}
