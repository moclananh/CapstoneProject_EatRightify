using Component.Application.Common;
using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.Catalog.Categories;
using Component.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace Component.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;

        public CategoryService(ApplicationDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<Category> Create(CategoryCreateRequest request)
        {
            var category = new Category()
            {
                Name = request.Name,
                Status = Data.Enums.Status.Active,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<int> Update(CategoryUpdateRequest request)
        {
            {
                var category = await _context.Categories.FindAsync(request.Id);

                if (category == null) throw new EShopException($"Cannot find a category with id: {request.Id}");
                category.Name = request.Name;
                category.Status = request.Status;
                return await _context.SaveChangesAsync();
            }
        }

        public async Task<int> Delete(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) throw new EShopException($"Cannot find a product: {categoryId}");

            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryVm>> GetAll(string? keyword)
        {
            var query = from c in _context.Categories
                        select new { c };
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.c.Name.Contains(keyword));
            }

            return await query.Select(x => new CategoryVm()
            {
                Id = x.c.Id,
                Name = x.c.Name,
            }).ToListAsync();
        }

        public async Task<CategoryVm> GetById(int id)
        {
            var query = from c in _context.Categories
                        where c.Id == id
                        select new { c };
            return await query.Select(x => new CategoryVm()
            {
                Id = x.c.Id,
                Name = x.c.Name,
                Status = x.c.Status
            }).FirstOrDefaultAsync();
        }

        public async Task<PagedResult<CategoryVm>> GetAllPaging(GetCategoryPagingRequest request)
        {
            //1. Select join
            var query = from c in _context.Categories
                        select new CategoryVm()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Status = c.Status
                        };
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.Name.Contains(request.Keyword));

            int totalRow = query.Count();

            var data = query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<CategoryVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }
    }
}
