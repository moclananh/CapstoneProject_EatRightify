﻿using Component.Application.Common;
using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.Catalog.ProductImages;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Component.Application.Catalog.Products
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        //private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public ProductService(ApplicationDbContext context, IStorageService storageService, IConfiguration configuration)
        {
            _context = context;
            _storageService = storageService;
            _configuration = configuration;
        }

        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            int imageId = 0;
            string path = "";
            string oldPath = _configuration["NoImage"];
            bool NoImage;
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder
            };
            var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault && i.ProductId == productId);
            path = thumbnailImage.ImagePath;
            NoImage = string.Equals(path, oldPath, StringComparison.OrdinalIgnoreCase);
            if ((!string.IsNullOrWhiteSpace(request.ImageFile) && IsBase64String(request.ImageFile)) && NoImage)
            {
                thumbnailImage.ImagePath = await _storageService.SaveImageAsync(request.ImageFile);
                thumbnailImage.FileSize = request.ImageFile.Length;
                _context.ProductImages.Update(thumbnailImage);
                imageId = thumbnailImage.Id;
            }
            else if ((!string.IsNullOrWhiteSpace(request.ImageFile) && IsBase64String(request.ImageFile)) && !NoImage)
            {
                productImage.ImagePath = await _storageService.SaveImageAsync(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
                _context.ProductImages.Add(productImage);
                await _context.SaveChangesAsync();
                imageId = productImage.Id;
            }
            else
            {
                productImage.ImagePath = thumbnailImage.ImagePath;
                productImage.FileSize = thumbnailImage.FileSize;
                _context.ProductImages.Update(thumbnailImage);
                imageId = thumbnailImage.Id;
            }

            await _context.SaveChangesAsync();
            return imageId;
        }

        public async Task AddViewcount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<Product> Create(ProductCreateRequest request)
        {
            var languages = _context.Languages;
            var translations = new List<ProductTranslation>();
            foreach (var language in languages)
            {
                if (language.Id == request.LanguageId)
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    });
                }/*
                else
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = SystemConstants.ProductConstants.NA,
                        Description = SystemConstants.ProductConstants.NA,
                        SeoAlias = SystemConstants.ProductConstants.NA,
                        LanguageId = language.Id
                    });
                }*/
            }
            var product = new Product()
            {
                Cost = request.Cost,
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                InputStock = request.InputStock,
                Stock = request.InputStock,
                ViewCount = 0,
                DateCreated = request.DateCreated,
                DateModified = request.DateCreated,
                IsFeatured = request.IsFeatured,
                ProductTranslations = translations,
                Status = Data.Enums.Status.Active
            };
            //Save image
            if (!string.IsNullOrWhiteSpace(request.ThumbnailImage) && IsBase64String(request.ThumbnailImage))
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await _storageService.SaveImageAsync(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1,
                    }
                };
            }
            else
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = 34648,
                        ImagePath = _configuration["NoImage"],
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product: {productId}");

            var images = _context.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }

            _context.Products.Remove(product);

            return await _context.SaveChangesAsync();
        }
        public async Task<PagedResult<ProductVm>> GetAllPaging(GetManageProductPagingRequest request)
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where pt.LanguageId == request.LanguageId
                        select new ProductVm()
                        {
                            Id = p.Id,
                            Name = pt.Name,
                            DateCreated = p.DateCreated,
                            Description = pt.Description,
                            Details = pt.Details,
                            LanguageId = pt.LanguageId,
                            OriginalPrice = p.OriginalPrice,
                            Price = p.Price,
                            SeoAlias = pt.SeoAlias,
                            SeoDescription = pt.SeoDescription,
                            SeoTitle = pt.SeoTitle,
                            Stock = p.Stock,
                            ViewCount = p.ViewCount,
                            IsFeatured = p.IsFeatured,
                            ThumbnailImage = pi.ImagePath,
                            Status = p.Status,
                            CategoryId = pic.CategoryId // Add CategoryId to ProductVm
                        };

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }

            // Filter by CategoryId
            if (request.CategoryId != null && request.CategoryId != 0)
            {
                query = query.Where(x => x.CategoryId == request.CategoryId);
            }

            // Create a list to store distinct products
            List<ProductVm> distinctProducts = new List<ProductVm>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.Id == productVm.Id))
                {
                    distinctProducts.Add(productVm);
                }
            }
            int totalRow = distinctProducts.Count();

            var data = distinctProducts
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<ProductVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };

            return pagedResult;
        }

        public async Task<ProductVm> GetById(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId);

            var categories = await (from c in _context.Categories
                                    join pic in _context.ProductInCategories on c.Id equals pic.CategoryId
                                    where pic.ProductId == productId
                                    select c.Name).ToListAsync();

            var comments = await (from c in _context.Comments
                                  join p in _context.Products on c.ProductId equals p.Id into pcmt
                                  from p in pcmt.DefaultIfEmpty()
                                  join u in _context.AppUsers on c.UserId equals u.Id into ucmt
                                  from u in ucmt.DefaultIfEmpty()
                                  where p.Id == productId
                                  select new CommentVm
                                  {
                                      Id = c.Id,
                                      UserId = c.UserId,
                                      UserName = u.UserName,
                                      ProductId = c.ProductId,
                                      Content = c.Content,
                                      CreatedAt = c.CreatedAt,
                                      Status = c.Status,
                                      Grade = c.Grade,
                                      UserAvatar = u.Avatar
                                  }).AsNoTracking().AsQueryable().ToListAsync(); // phai co asNoTracking de ignore vong lap vo hang

            var image = await _context.ProductImages.Where(x => x.ProductId == productId && x.IsDefault == true).FirstOrDefaultAsync();

            var productViewModel = new ProductVm()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation != null ? productTranslation.Description : null,
                LanguageId = productTranslation.LanguageId,
                Details = productTranslation != null ? productTranslation.Details : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                Status = product.Status,
                Categories = categories,
                CommentsList = comments,
                IsFeatured = product.IsFeatured,
                ThumbnailImage = image != null ? image.ImagePath : "no-image.jpg",
                InputStock = product.InputStock,
                Cost = product.Cost,
                DateModified = product.DateModified,
            };
            return productViewModel;
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                throw new EShopException($"Cannot find an image with id {imageId}");

            var viewModel = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                Id = image.Id,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };
            return viewModel;
        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            return await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(i => new ProductImageViewModel()
                {
                    Caption = i.Caption,
                    DateCreated = i.DateCreated,
                    FileSize = i.FileSize,
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    IsDefault = i.IsDefault,
                    ProductId = i.ProductId,
                    SortOrder = i.SortOrder
                }).ToListAsync();
        }

        public async Task<int> RemoveImage(int imageId)
        {
            int idPro = 0;
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new EShopException($"Cannot find an image with id {imageId}");
            idPro = productImage.ProductId;
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault && i.ProductId == idPro);
            if (thumbnailImage == null)
            {
                var newThumbnailImage = new ProductImage
                {

                    Caption = "Thumbnail image",
                    DateCreated = DateTime.Now,
                    FileSize = 34648,
                    ImagePath = _configuration["NoImage"],
                    IsDefault = true,
                    SortOrder = 1,
                    ProductId = idPro
                };

                _context.ProductImages.Add(newThumbnailImage);
            }
            else
            {
                _context.ProductImages.Update(thumbnailImage);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslations = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id
            && x.LanguageId == request.LanguageId);

            if (product == null || productTranslations == null) throw new EShopException($"Cannot find a product with id: {request.Id}");

            productTranslations.Name = request.Name;
            productTranslations.SeoAlias = request.SeoAlias;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.SeoTitle = request.SeoTitle;
            productTranslations.Description = request.Description;
            productTranslations.Details = request.Details;
            product.IsFeatured = request.IsFeatured;
            product.Status = request.Status;
            product.Price = request.Price;
            product.OriginalPrice = request.OriginalPrice;
            int temp = request.Stock - product.Stock;
            product.InputStock += temp;
            product.Stock = request.Stock;
            product.DateModified = request.DateModified;
            product.Cost = request.Cost;

            //Save image
            // Check if ThumbnailImage is a base64-encoded string
            if (!string.IsNullOrWhiteSpace(request.ThumbnailImage) && IsBase64String(request.ThumbnailImage))
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault && i.ProductId == request.Id);
                // Update existing image properties
                thumbnailImage.FileSize = request.ThumbnailImage.Length;
                thumbnailImage.ImagePath = await _storageService.SaveImageAsync(request.ThumbnailImage);
                _context.ProductImages.Update(thumbnailImage);
            }
            else
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault && i.ProductId == request.Id);
                request.ThumbnailImage = thumbnailImage.ImagePath;
                _context.ProductImages.Update(thumbnailImage);
            }
            return await _context.SaveChangesAsync();
        }

        // Helper method to check if a string is a base64-encoded string
        private bool IsBase64String(string s)
        {
            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
                throw new EShopException($"Cannot find an image with id {imageId}");

            if (!string.IsNullOrWhiteSpace(request.ImageFile) && IsBase64String(request.ImageFile))
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault && i.ProductId == request.Id);
                // Update existing image properties
                thumbnailImage.FileSize = request.ImageFile.Length;
                thumbnailImage.ImagePath = await _storageService.SaveImageAsync(request.ImageFile);
                _context.ProductImages.Update(thumbnailImage);
            }
            else
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault && i.ProductId == request.Id);
                request.ImageFile = thumbnailImage.ImagePath;
                _context.ProductImages.Update(thumbnailImage);
            }
            _context.ProductImages.Update(productImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product with id: {productId}");
            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product with id: {productId}");
            product.Stock += addedQuantity;
            return await _context.SaveChangesAsync() > 0;
        }

        /*   private async Task<string> SaveFile(IFormFile file)
           {
               var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
               var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
               await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
               return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
           }
*/
        public async Task<PagedResult<ProductVm>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request)
        {
            //1. Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };
            //2. filter
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }
            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    IsFeatured = x.p.IsFeatured,
                    ViewCount = x.p.ViewCount,
                    Status = x.p.Status,
                }).ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResult<ProductVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var user = await _context.Products.FindAsync(id);
            if (user == null)
            {
                return new ApiErrorResult<bool>($"Sản phẩm với id {id} không tồn tại");
            }

            List<SelectItem> mang = new List<SelectItem>();

            foreach (var item in request.Categories)
            {
                mang.Add(item);
                // Check if the product with the same ID is already in the distinctProducts list
            }

            foreach (var category in mang)
            {
                var productInCategory = await _context.ProductInCategories
                    .FirstOrDefaultAsync(x => x.CategoryId == int.Parse(category.Id)
                    && x.ProductId == id);
                if (productInCategory != null && category.Selected == false)
                {
                    _context.ProductInCategories.Remove(productInCategory);
                }
                else if (productInCategory == null && category.Selected)
                {
                    await _context.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        CategoryId = int.Parse(category.Id),
                        ProductId = id
                    });
                }
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<List<ProductVm>> GetFeaturedProducts()
        {
            var query = await (from p in _context.Products
                         join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                         join pic in _context.ProductInCategories on p.Id equals pic.ProductId into ppic
                         from pic in ppic.DefaultIfEmpty()
                         join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                         from pi in ppi.DefaultIfEmpty()
                         join c in _context.Categories on pic.CategoryId equals c.Id into picc
                         from c in picc.DefaultIfEmpty()
                         where p.IsFeatured == true && p.Status == Data.Enums.Status.Active
                         select new { p, pt, pic, pi })
                        .Select(x => new ProductVm()
                        {
                            Id = x.p.Id,
                            Name = x.pt.Name,
                            DateCreated = x.p.DateCreated,
                            Description = x.pt.Description,
                            Details = x.pt.Details,
                            LanguageId = x.pt.LanguageId,
                            OriginalPrice = x.p.OriginalPrice,
                            Price = x.p.Price,
                            SeoAlias = x.pt.SeoAlias,
                            SeoDescription = x.pt.SeoDescription,
                            SeoTitle = x.pt.SeoTitle,
                            Stock = x.p.Stock,
                            ViewCount = x.p.ViewCount,
                            IsFeatured = x.p.IsFeatured,
                            Status = x.p.Status,
                            ThumbnailImage = x.pi.ImagePath
                        }).Distinct().ToListAsync();
            var result = query.OrderByDescending(x => x.Id).Take(4).ToList();
            return result;
        }


        public async Task<List<ProductVm>> GetLatestProducts(string languageId, int take)
        {
            //1. Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi == null || pi.IsDefault == true)
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take) // stupid take() nen phai dung trick trong Utilities
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    IsFeatured = x.p.IsFeatured,
                    Status = x.p.Status,
                    ThumbnailImage = x.pi.ImagePath
                }).Distinct().ToListAsync();

            return data;
        }

        public async Task<List<ProductVm>> GetAll(GetAllProductRequest request)
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where pt.LanguageId == request.LanguageId
                        select new ProductVm()
                        {
                            Id = p.Id,
                            Name = pt.Name,
                            DateCreated = p.DateCreated,
                            Description = pt.Description,
                            Details = pt.Details,
                            LanguageId = pt.LanguageId,
                            OriginalPrice = p.OriginalPrice,
                            Price = p.Price,
                            SeoAlias = pt.SeoAlias,
                            SeoDescription = pt.SeoDescription,
                            SeoTitle = pt.SeoTitle,
                            Stock = p.Stock,
                            ViewCount = p.ViewCount,
                            IsFeatured = p.IsFeatured,
                            ThumbnailImage = pi.ImagePath,
                            Status = p.Status,
                            CategoryId = pic.CategoryId,
                            InputStock = p.InputStock,
                            Cost = p.Cost,
                            DateModified = p.DateModified,
                        };

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }

            // Filter by CategoryId
            if (request.CategoryId != null && request.CategoryId != 0)
            {
                query = query.Where(x => x.CategoryId == request.CategoryId);
            }

            // Create a list to store distinct products
            List<ProductVm> distinctProducts = new List<ProductVm>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.Id == productVm.Id) )
                {
                    distinctProducts.Add(productVm);
                }
            }

            var queryFilter = distinctProducts
             .OrderByDescending(item => item.DateCreated)
             .ToList();

            return queryFilter;
        }

        public async Task<string> CreateBase64Image(IFormFile image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                string base64Image = Convert.ToBase64String(imageBytes);
                return base64Image;
            }
        }

        public async Task<decimal> SumOfCost(DateTime? startDate, DateTime? endDate)
        {
            var query = from p in _context.Products
                        select new ProductVm()
                        {
                            Id = p.Id,
                            DateCreated = p.DateCreated,
                            OriginalPrice = p.OriginalPrice,
                            Price = p.Price,
                            Stock = p.Stock,
                            ViewCount = p.ViewCount,
                            IsFeatured = p.IsFeatured,
                            Status = p.Status,
                            InputStock = p.InputStock,
                            Cost = p.Cost,
                            DateModified = p.DateModified,
                        };

            //2. filter
            if (startDate != null && endDate != null)
            {
                query = query.Where(x => x.DateCreated >= startDate && x.DateCreated <= endDate);
            }

            List<ProductVm> distinctProducts = new List<ProductVm>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.Id == productVm.Id) /*&& !(productVm.Name == "N/A")*/)
                {
                    distinctProducts.Add(productVm);
                }
            }

            // Tính tổng Cost nhân với InputStock tương ứng
            decimal sumOfCost = distinctProducts
                .Select(x => x.Cost * x.InputStock) // Nhân giá trị Cost với InputStock của mỗi sản phẩm
                .DefaultIfEmpty(0) // Nếu không có sản phẩm nào, mặc định là 0
                .Sum(); // Tính tổng của các giá trị

            return sumOfCost;
        }

        public async Task<List<ProductVm>> GetProductForAI()
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where p.Status == Data.Enums.Status.Active
                        select new ProductVm()
                        {
                            Id = p.Id,
                            Name = pt.Name,
                            DateCreated = p.DateCreated,
                            Description = pt.Description,
                            Details = pt.Details,
                            LanguageId = pt.LanguageId,
                            OriginalPrice = p.OriginalPrice,
                            Price = p.Price,
                            SeoAlias = pt.SeoAlias,
                            SeoDescription = pt.SeoDescription,
                            SeoTitle = pt.SeoTitle,
                            Stock = p.Stock,
                            ViewCount = p.ViewCount,
                            IsFeatured = p.IsFeatured,
                            ThumbnailImage = pi.ImagePath,
                            Status = p.Status,
                            CategoryId = pic.CategoryId, // Add CategoryId to ProductVm
                            InputStock = p.InputStock,
                            Cost = p.Cost,
                            DateModified = p.DateModified,
                        };
            // Create a list to store distinct products
            List<ProductVm> distinctProducts = new List<ProductVm>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.Id == productVm.Id) /*&& !(productVm.Name == "N/A")*/)
                {
                    distinctProducts.Add(productVm);
                }
            }

            var queryFilter = distinctProducts
             .OrderByDescending(item => item.DateCreated) // Sort by TotalQuantity in descending order
             .ToList();

            return queryFilter;
        }

        public async Task<int> TotalView()
        {
            var totalViewCount = await (from p in _context.Products
                                        select p.ViewCount).SumAsync();
            return totalViewCount;
        }

        public async Task<List<ProductVm>> GetAllProductActive(GetAllProductRequest request)
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where pt.LanguageId == request.LanguageId && p.Status == Data.Enums.Status.Active
                        select new ProductVm()
                        {
                            Id = p.Id,
                            Name = pt.Name,
                            DateCreated = p.DateCreated,
                            Description = pt.Description,
                            Details = pt.Details,
                            LanguageId = pt.LanguageId,
                            OriginalPrice = p.OriginalPrice,
                            Price = p.Price,
                            SeoAlias = pt.SeoAlias,
                            SeoDescription = pt.SeoDescription,
                            SeoTitle = pt.SeoTitle,
                            Stock = p.Stock,
                            ViewCount = p.ViewCount,
                            IsFeatured = p.IsFeatured,
                            ThumbnailImage = pi.ImagePath,
                            Status = p.Status,
                            CategoryId = pic.CategoryId,
                            InputStock = p.InputStock,
                            Cost = p.Cost,
                            DateModified = p.DateModified,
                        };

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }

            // Filter by CategoryId
            if (request.CategoryId != null && request.CategoryId != 0)
            {
                query = query.Where(x => x.CategoryId == request.CategoryId);
            }

            // Create a list to store distinct products
            List<ProductVm> distinctProducts = new List<ProductVm>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.Id == productVm.Id))
                {
                    distinctProducts.Add(productVm);
                }
            }

            var queryFilter = distinctProducts
             .OrderByDescending(item => item.DateCreated)
             .ToList();

            return queryFilter;
        }

        public async Task<int> ReStock(int productId, int stock)
        {
            var product = await _context.Products.FindAsync(productId);
            product.Stock += stock;
            return await _context.SaveChangesAsync();
        }
    }
}
