using Component.Data.EF;
using Component.Data.Entities;
using Component.Utilities.Exceptions;
using Component.ViewModels.Common;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Comments;
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
            var promotion = new Promotion()
            {
                DiscountCode = Guid.NewGuid(),
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                DiscountPercent = request.DiscountPercent,
                Status = Data.Enums.Status.Active,
                Name = request.Name,
                Description = request.Description
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        } 

        public async Task<int> Delete(int promotionId)
        {
            var promotion = await _context.Promotions.FindAsync(promotionId);
            if (promotion == null) throw new EShopException($"Cannot find a promotion: {promotionId}");

            _context.Promotions.Remove(promotion);
            return await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<PromotionVm>> GetAllPaging(GetPromotionPagingRequest request)
        {
            var query = from p in _context.Promotions
                        select new PromotionVm()
                        {
                           Id = p.Id,
                           DiscountCode = p.DiscountCode,
                           FromDate = p.FromDate,
                           ToDate = p.ToDate,
                           DiscountPercent = p.DiscountPercent,
                           Status = p.Status,
                           Name = p.Name,
                           Description = p.Description
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
                        where p.Id == id
                        select new { p };
            return await query.Select(x => new PromotionVm()
            {
                Id = x.p.Id,
                DiscountCode = x.p.DiscountCode,
                FromDate = x.p.FromDate,
                ToDate = x.p.ToDate,
                DiscountPercent = x.p.DiscountPercent,
                Status = x.p.Status,
                Name = x.p.Name,
                Description = x.p.Description
            }).FirstOrDefaultAsync();
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
