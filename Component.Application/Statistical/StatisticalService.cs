using Component.Data.EF;
using Component.Data.Entities;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.Statistical;
using Component.ViewModels.Utilities.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Component.Application.Statistical
{
    public class StatisticalService : IStatisticalService
    {
        private readonly ApplicationDbContext _context;
       // private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public StatisticalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StatisticalVm>> GetAll(StatisticalRequest request)
        {
            var query = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.Id
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pimg in _context.ProductImages on p.Id equals pimg.ProductId
                        where (o.OrderDate >= request.StartDate && o.OrderDate <= request.EndDate)
                        group new { od, pt, pimg } by new
                        {
                            od.ProductId,
                            pt.Name,
                            pimg.ImagePath
                        }
                      into grouped
                        select new StatisticalVm()
                        {
                            ProductId = grouped.Key.ProductId,
                            Name = grouped.Key.Name,
                            ImagePath = grouped.Key.ImagePath,
                            TotalQuantity = grouped.Sum(item => item.od.Quantity),
                            TotalPrice = (float)grouped.Sum(item => item.od.Price)
                        };

            // Create a list to store distinct products
            List<StatisticalVm> distinctProducts = new List<StatisticalVm>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.ProductId == productVm.ProductId) /*&& !(productVm.Name == "N/A")*/)
                {
                    distinctProducts.Add(productVm);
                }
            }


            var queryFilter = distinctProducts
              .Where(item => item.Name != "N/A")
              .OrderByDescending(item => item.TotalQuantity) // Sort by TotalQuantity in descending order
              .ToList();


            return queryFilter;
        }

        public async Task<List<ProductInteractionRequest>> GetListProductInteractions(string keyword)
        {

            var query = from c in _context.Comments
                         join u in _context.AppUsers on c.UserId equals u.Id
                         join p in _context.Products on c.ProductId equals p.Id
                         join pimg in _context.ProductImages on p.Id equals pimg.ProductId 
                         join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                         group new { c } by new
                         {
                             c.ProductId,
                             pt.Name,
                             pimg.ImagePath
                         }
                        into grouped
                         select new ProductInteractionRequest()
                         { 
                             ProductId = grouped.Key.ProductId,
                             ProductName = grouped.Key.Name,
                             ImagePath = grouped.Key.ImagePath,
                             TotalOfComment = grouped.Count()
                        };

            // filter
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.ProductName.Contains(keyword));

            // Create a list to store distinct products
            List<ProductInteractionRequest> distinctProducts = new List<ProductInteractionRequest>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.ProductId == productVm.ProductId) /*&& !(productVm.Name == "N/A")*/)
                {
                    distinctProducts.Add(productVm);
                }
            }

            var queryFilter = distinctProducts
              .OrderByDescending(item => item.TotalOfComment) 
              .ToList();

            return queryFilter;
        }

        public async Task<List<UserInteractionRequest>> GetListUserInteractions(string keyword)
        {
            var query = from c in _context.Comments
                        join u in _context.AppUsers on c.UserId equals u.Id
                        group new { c, u} by new
                        {
                            u.UserName,
                            u.Id
                        }
                         into grouped
                        select new UserInteractionRequest()
                        {
                            UserName = grouped.Key.UserName,
                            UserId = grouped.Key.Id,
                            TotalOfComment = grouped.Count()
                        };

            // filter
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.UserName.Contains(keyword));

            // Create a list to store distinct products
            List<UserInteractionRequest> distinctProducts = new List<UserInteractionRequest>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.UserId == productVm.UserId))
                {
                    distinctProducts.Add(productVm);
                }
            }

            var queryFilter = distinctProducts
              .OrderByDescending(item => item.TotalOfComment)
              .ToList();

            return queryFilter;
        }

        public async Task<PagedResult<StatisticalVm>> Statistical(StatisticalPagingRequest request)
        {
            var query = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.Id
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pimg in _context.ProductImages on p.Id equals pimg.ProductId
                        where (o.OrderDate >= request.StartDate && o.OrderDate <= request.EndDate)
                        group new { od, pt, pimg } by new
                        {
                            od.ProductId,
                            pt.Name,
                            pimg.ImagePath
                        }
                        into grouped
                        select new StatisticalVm()
                        {
                            ProductId = grouped.Key.ProductId,
                            Name = grouped.Key.Name,
                            ImagePath = grouped.Key.ImagePath,
                            TotalQuantity = grouped.Sum(item => item.od.Quantity),
                            TotalPrice = (float)grouped.Sum(item => item.od.Price)
                        };

            // Create a list to store distinct products
            List<StatisticalVm> distinctProducts = new List<StatisticalVm>();

            foreach (var productVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.ProductId == productVm.ProductId) /*&& !(productVm.Name == "N/A")*/)
                {
                    distinctProducts.Add(productVm);
                }
            }



            var queryFilter = distinctProducts
              .Where(item => item.Name != "N/A")
              .OrderByDescending(item => item.TotalQuantity) // Sort by TotalQuantity in descending order
              .ToList();

            int totalRow = queryFilter.Count();

            var data = queryFilter
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<StatisticalVm>()
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
