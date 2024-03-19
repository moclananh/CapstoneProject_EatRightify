using Component.Data.Entities;
using Component.ViewModels.Catalog.ProductImages;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Catalog.Products
{
    public interface IProductService
    {
        Task<Product> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<ProductVm> GetById(int productId);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task AddViewcount(int productId);

        Task<PagedResult<ProductVm>> GetAllPaging(GetManageProductPagingRequest request);

        Task<int> AddImage(int productId, ProductImageCreateRequest request);

        Task<int> RemoveImage(int imageId);

        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request);

        Task<ProductImageViewModel> GetImageById(int imageId);

        Task<List<ProductImageViewModel>> GetListImages(int productId);

        Task<PagedResult<ProductVm>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request);

        Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request);

        Task<List<ProductVm>> GetFeaturedProducts(int take);

        Task<List<ProductVm>> GetLatestProducts(string languageId, int take);
        Task<List<ProductVm>> GetAll(GetAllProductRequest request);
        Task<List<ProductVm>> GetAllProductActive(GetAllProductRequest request);
        Task<List<ProductVm>> GetProductForAI();
        Task<string> CreateBase64Image(IFormFile image);
        Task<decimal> SumOfCost (DateTime? startDate, DateTime? endDate);
        Task<int> TotalView();
        Task<int> ReStock(int productId, int stock);
    }
}
