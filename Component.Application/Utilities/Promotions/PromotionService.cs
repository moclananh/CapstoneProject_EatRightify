using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Orders;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Comments;
using Component.ViewModels.Utilities.Locations;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Utilities.Promotions
{
    public class PromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _context;
        public PromotionService(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<Promotion> Create(PromotionCreateRequest request)
        {
            Random generator = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randomCode = new string(Enumerable.Repeat(chars, 11)
              .Select(s => s[generator.Next(s.Length)]).ToArray());

            var promotion = new Promotion()
            {
                DiscountCode = randomCode,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                DiscountPercent = request.DiscountPercent,
                Status = Data.Enums.Status.Active,
                Name = request.Name,
                Description = request.Description,
                CreatedBy = request.CreatedBy,
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<int> Delete(int promotionId)
        {
            var check = await GetById(promotionId);
            var promotion = await _context.Promotions.FirstOrDefaultAsync(x => x.Id == promotionId);
            if (promotion == null)
            {
                throw new EShopException($"Cannot find a location: {promotionId}");
            }
            if (check.Id != promotion.Id)
            {
                throw new EShopException($"Error to find location: {promotionId}");
            }
            _context.Promotions.Remove(promotion);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<PromotionVm>> GetAll(string keyword)
        {
            var query = from p in _context.Promotions
                        join u in _context.AppUsers on p.CreatedBy equals u.Id into pu
                        from u in pu.DefaultIfEmpty()
                        select new { p, u };

            //filter
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.p.Name.Contains(keyword));

            var result = await query.Select(x => new PromotionVm()
            {
                Id = x.p.Id,
                DiscountCode= x.p.DiscountCode,
                FromDate = x.p.FromDate,
                ToDate= x.p.ToDate,
                DiscountPercent= x.p.DiscountPercent,
                Status= x.p.Status,
                Name= x.p.Name,
                Description= x.p.Description,
                CreatedBy = x.u.UserName
            }).Distinct().ToListAsync();
            result = result.OrderByDescending(x => x.FromDate).ToList();
            return result;
        }

        public async Task<PagedResult<PromotionVm>> GetAllPaging(GetPromotionPagingRequest request)
        {
            var query = from p in _context.Promotions
                        join u in _context.AppUsers on p.CreatedBy equals u.Id into pu
                        from u in pu.DefaultIfEmpty()
                        select new PromotionVm()
                        {
                           Id = p.Id,
                           DiscountCode = p.DiscountCode,
                           FromDate = p.FromDate,
                           ToDate = p.ToDate,
                           DiscountPercent = p.DiscountPercent,
                           Status = p.Status,
                           Name = p.Name,
                           Description = p.Description,
                           CreatedBy= u.UserName,
                        };
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.Name.Contains(request.Keyword));


            int totalRow = query.Count();

            var data = query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<PromotionVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<PromotionVm> GetById(int id)
        {
            var query = from p in _context.Promotions
                        join u in _context.AppUsers on p.CreatedBy equals u.Id into pu
                        from u in pu.DefaultIfEmpty()
                        where p.Id == id
                        select new { p, u };
            return await query.Select(x => new PromotionVm()
            {
                Id = x.p.Id,
                DiscountCode = x.p.DiscountCode,
                FromDate = x.p.FromDate,
                ToDate = x.p.ToDate,
                DiscountPercent = x.p.DiscountPercent,
                Status = x.p.Status,
                Name = x.p.Name,
                Description = x.p.Description,
                CreatedBy = x.u.UserName,
            }).FirstOrDefaultAsync();
        }

        public async Task<ApiResult<PromotionVm>> GetByPromotionCode(string code)
        {
            var query = from p in _context.Promotions
                        join u in _context.AppUsers on p.CreatedBy equals u.Id into pu
                        from u in pu.DefaultIfEmpty()
                        where p.DiscountCode.Equals(code)
                        select new { p, u };

            var promotion = await query.FirstOrDefaultAsync(); // Lấy thông tin khuyến mãi
            if (promotion == null)
            {
                return new ApiErrorResult<PromotionVm>("Voucher does not exist!");
            }

            var timeNow = DateTime.Now;

            if (timeNow < promotion.p.FromDate || timeNow > promotion.p.ToDate)
            {
                return new ApiErrorResult<PromotionVm>("Voucher expire!");
            }
            var result = await query.Select(x => new PromotionVm()
            {
                Id = x.p.Id,
                DiscountCode = x.p.DiscountCode,
                FromDate = x.p.FromDate,
                ToDate = x.p.ToDate,
                DiscountPercent = x.p.DiscountPercent,
                Status = x.p.Status,
                Name = x.p.Name,
                Description = x.p.Description,
                CreatedBy = x.u.UserName
            }).FirstOrDefaultAsync();
            return new ApiSuccessResult<PromotionVm>(result);
        }


        public async Task<int> Update(PromotionUpdateRequest request)
        {
            var promotions = await _context.Promotions.FindAsync(request.Id);

            if (promotions == null) throw new EShopException($"Cannot find a promotion with id: {request.Id}");
            promotions.FromDate = request.FromDate;
            promotions.ToDate = request.ToDate;
            promotions.DiscountPercent = request.DiscountPercent;
            promotions.Status = request.Status;
            promotions.Name = request.Name;
            promotions.Description = request.Description;
           
            return await _context.SaveChangesAsync();
        }
    }
}
