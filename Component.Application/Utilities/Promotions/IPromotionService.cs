using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Promotions
{
    public interface IPromotionService
    {
        Task<PagedResult<PromotionVm>> GetAllPaging(GetPromotionPagingRequest request);
        Task<PromotionVm> GetById(int id);
        Task<PromotionVm> GetByPromotionCode(Guid code);
        Task<Promotion> Create(PromotionCreateRequest request);
        Task<int> Update(PromotionUpdateRequest request);
        Task<int> Delete(int promotionId);
    }
}
