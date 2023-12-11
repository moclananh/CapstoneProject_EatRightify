using Component.ViewModels.AI;
using Component.ViewModels.Common;

namespace Component.Application.AI
{
    public interface IAIService
    {
        Task<UpdateResultRequest> Update(int id, UpdateResultRequest request);
        Task<UpdateStatusResult> UpdateStatus(int id, UpdateStatusResult status);
        Task<PagedResult<ResultVM>> GetAllPaging(ResultPagingRequest request);
        Task<ResultVM> GetById(int id);
        Task<int> Delete(int id);
    }
}
