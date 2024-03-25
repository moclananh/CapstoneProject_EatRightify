using Component.Data.Entities;
using Component.Data.Enums;
using Component.ViewModels.AI;
using Component.ViewModels.Common;

namespace Component.Application.AI
{
    public interface IAIService
    {
        Task<UpdateResultRequest> Update(int id, UpdateResultRequest request);
        Task<UpdateStatusResult> UpdateStatus(int id, UpdateStatusResult status);
        Task<int> UpdateIsSend(int id, UpdateIsSendRequest request);
        Task<List<ResultVM>> GetAll(string keyword, ResultStatus? status);
        Task<ResultVM> GetById(int id);
        Task<ResultVM> GetByUserId(Guid userId);
        Task<int> Delete(int id);
        Task<Result> Create(CreateResultRequest request);
        Task<ApiResult<string>> GetResultEmail(string email, int id);
        // Task<PagedResult<ResultVM>> GetAllPaging(ResultPagingRequest request);
    }
}
