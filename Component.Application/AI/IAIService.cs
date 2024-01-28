using Component.Data.Entities;
using Component.ViewModels.AI;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Promotions;

namespace Component.Application.AI
{
    public interface IAIService
    {
        Task<UpdateResultRequest> Update(int id, UpdateResultRequest request);
        Task<UpdateStatusResult> UpdateStatus(int id, UpdateStatusResult status);
        Task<int> UpdateIsSend(int id, UpdateIsSendRequest request);
        Task<PagedResult<ResultVM>> GetAllPaging(ResultPagingRequest request);
        Task<List<ResultVM>> GetAll(string keyword);
        Task<ResultVM> GetById(int id);
        Task<ResultVM> GetByUserId(Guid userId);
        Task<int> Delete(int id);
        Task<Result> Create(CreateResultRequest request);
        Task<ApiResult<string>> GetResultEmail(string email, int id);
    }
}
