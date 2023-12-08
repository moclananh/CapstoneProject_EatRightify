using Component.ViewModels.AI;

namespace Component.Application.AI
{
    public interface IAIService
    {
        Task<UpdateResultRequest> Update(int id, UpdateResultRequest request);
        Task<UpdateStatusResult> UpdateStatus(int id, UpdateStatusResult status);
    }
}
