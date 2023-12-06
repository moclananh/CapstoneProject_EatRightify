using Component.Data.Entities;
using Component.ViewModels.Catalog.Categories;
using Component.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryVm>> GetAll(string languageId);
        Task<PagedResult<CategoryVm>> GetAllPaging(GetCategoryPagingRequest request);

        Task<CategoryVm> GetById(string languageId, int id);
        Task<Category> Create(CategoryCreateRequest request);

        Task<int> Update(CategoryUpdateRequest request);

        Task<int> Delete(int categoryId);
    }
}
