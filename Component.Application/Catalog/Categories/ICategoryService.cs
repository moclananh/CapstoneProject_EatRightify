using Component.Data.Entities;
using Component.ViewModels.Catalog.Categories;

namespace Component.Application.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryVm>> GetAll(string? keyword);
        Task<CategoryVm> GetById(int id);
        Task<Category> Create(CategoryCreateRequest request);
        Task<int> Update(CategoryUpdateRequest request);
        Task<int> Delete(int categoryId);
        //Task<PagedResult<CategoryVm>> GetAllPaging(GetCategoryPagingRequest request);
    }
}
