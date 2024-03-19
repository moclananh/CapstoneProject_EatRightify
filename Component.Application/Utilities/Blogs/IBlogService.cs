using Component.Data.Entities;
using Component.ViewModels.Catalog.Categories;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Blogs
{
    public interface IBlogService
    {
        Task<List<BlogVm>> GetAll();
        Task<List<BlogVm>> GetAllBlogActive();
        Task<PagedResult<BlogVm>> GetAllPaging(GetBlogPagingRequest request);

        Task<BlogVm> GetById(int id);
        Task<Blog> Create(BlogCreateRequest request);

        Task<int> Update(BlogUpdateRequest request);
        Task AddViewcount(int blogId);
        Task<int> Delete(int blogId);
        Task<int> TotalView();
    }
}
