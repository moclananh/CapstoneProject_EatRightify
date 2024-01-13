using Component.Data.Entities;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Comments;
using Component.ViewModels.Utilities.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Comments
{
    public interface ICommentService
    {
        Task<PagedResult<CommentVm>> GetAllCommentByProductIdPaging(GetCommentPagingRequest request);
        Task<List<CommentVm>> GetAll(int productId);
        Task<CommentVm> GetById(int id);
        Task<Comment> Create(CommentCreateRequest request);

        Task<int> Update(CommentUpdateRequest request);

        Task<int> Delete(int commentId);
    }
}
