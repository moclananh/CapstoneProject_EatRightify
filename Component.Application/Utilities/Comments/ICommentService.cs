using Component.Data.Entities;
using Component.ViewModels.Utilities.Comments;

namespace Component.Application.Utilities.Comments
{
    public interface ICommentService
    {
        Task<List<CommentVm>> GetAll(int productId);
        Task<CommentVm> GetById(int id);
        Task<Comment> Create(CommentCreateRequest request);
        Task<int> Update(CommentUpdateRequest request);
        Task<int> Delete(int commentId);
        //Task<PagedResult<CommentVm>> GetAllCommentByProductIdPaging(GetCommentPagingRequest request);
    }
}
