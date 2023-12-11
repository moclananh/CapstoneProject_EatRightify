using Component.Data.EF;
using Component.Data.Entities;
using Component.Data.Enums;
using Component.Utilities.Exceptions;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Bills;
using Component.ViewModels.Sales.Orders;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.Sales
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Order> Create(CheckoutRequest request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var order = new Order()
                    {
                        OrderDate = DateTime.Now,
                        UserId = request.UserId,
                        ShipName = request.Name,
                        ShipAddress = request.Address,
                        ShipEmail = request.Email,
                        ShipPhoneNumber = request.PhoneNumber,
                        Status = OrderStatus.InProgress,
                        OrderCode = Guid.NewGuid(),
                        OrderDetails = new List<OrderDetail>() { }
                    };

                    foreach (var orderDetailVm in request.OrderDetails)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDetailVm.ProductId);

                        if (product != null)
                        {
                            var orderDetail = new OrderDetail()
                            {
                                ProductId = product.Id,
                                Quantity = orderDetailVm.Quantity,
                                Price = product.Price * orderDetailVm.Quantity
                            };

                            if (orderDetail.Quantity <= product.Stock)
                            {
                                order.OrderDetails.Add(orderDetail);
                                await UpdateStockCheckout(product.Id, orderDetailVm.Quantity);
                            }
                            else
                            {
                                // If any product has a quantity greater than stock, rollback the transaction
                                transaction.Rollback();
                                return null;
                            }
                        }
                        else
                        {
                            // If any product is not found, rollback the transaction
                            transaction.Rollback();
                            throw new Exception($"Product with ID {orderDetailVm.ProductId} not found.");
                        }
                    }

                    // If all products are valid, save the changes
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    transaction.Commit();

                    return order;
                }
                catch (Exception ex)
                {
                    // Handle exceptions here if needed
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public async Task<Order> GetById(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            return order;
        }

        public async Task<Order> GetLastestOrderId()
        {
            var latestOrder = await _context.Orders.OrderByDescending(order => order.Id).FirstOrDefaultAsync();

            if (latestOrder == null)
            {
                throw new Exception("No orders found in the database.");
            }

            return latestOrder;
        }

        public async Task<PagedResult<OrderVm>> GetAllPaging(OrderPagingRequest request)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId

                        select new OrderVm()
                        {
                            Id = o.Id,
                            OrderDate = o.OrderDate,
                            UserId = o.UserId,
                            ShipName = o.ShipName,
                            ShipAddress = o.ShipAddress,
                            ShipEmail = o.ShipEmail,
                            ShipPhoneNumber = o.ShipPhoneNumber,
                            Status = o.Status,
                            OrderCode = o.OrderCode,
                            OrderDetails = new List<OrderDetail>()

                        };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.ShipPhoneNumber.Contains(request.Keyword));

            // Create a list to store distinct products
            List<OrderVm> distinctOrder = new List<OrderVm>();

            foreach (var orderVm in query)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctOrder.Any(p => p.Id == orderVm.Id))
                {
                    distinctOrder.Add(orderVm);
                }
            }
            int totalRow = distinctOrder.Count();

            var data = distinctOrder
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<OrderVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<PagedResult<OrderDetailView>> GetOrderDetailPagingRequest(OrderDetailPagingRequest request)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join l in _context.Languages on pt.LanguageId equals l.Id
                        where o.Id == request.OrderId
                        select new OrderDetailView()
                        {
                            ProductName = pt.Name,
                            Quatity = od.Quantity,
                            Price = od.Price
                        };


            var queryFilter = query.Where(item => item.ProductName != "N/A").ToList();
            int totalRow = queryFilter.Count();

            var data = queryFilter
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedResult = new PagedResult<OrderDetailView>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }
        public async Task<BillHistoryDetailVM> GetBillById(int id)
        {
            var result = new BillHistoryDetailVM();

            var query = await (from o in _context.Orders
                               join a in _context.AppUsers on o.UserId equals a.Id
                               join od in _context.OrderDetails on o.Id equals od.OrderId
                               join p in _context.Products on od.ProductId equals p.Id
                               join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                               where o.Id == id
                               select new
                               {
                                   ID = o.Id,
                                   Name = pt.Name,
                                   Quantity = od.Quantity,
                                   Price = p.Price,
                                   Status = o.Status
                               }).ToListAsync();

            // Filter out entries with Name = "N/A"
            query = query.Where(item => item.Name != "N/A").ToList();

            if (query.Any())
            {
                result.ID = query.First().ID;
                result.status = query.First().Status;

                // Populate the lists
                result.name = query.Select(item => item.Name).ToList();
                result.quantity = query.Select(item => item.Quantity).ToList();
                result.price = query.Select(item => item.Price).ToList();
            }

            return result;
        }

        public async Task<List<BillHistoryVM>> BillHistory(Guid id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            var result = new List<BillHistoryVM>();

            var query = await (from o in _context.Orders
                               join a in _context.AppUsers on o.UserId equals a.Id
                               join od in _context.OrderDetails on o.Id equals od.OrderId
                               join p in _context.Products on od.ProductId equals p.Id
                               where o.UserId == id
                               select new BillHistoryVM
                               {
                                   id = o.Id,
                                   email = a.Email,
                                   address = o.ShipAddress,
                                   orderDate = o.OrderDate,
                                   price = od.Price,
                                   Status = o.Status
                               }).ToListAsync();

            // Group the results by id and calculate the sum of price
            var groupedQuery = query
                .GroupBy(item => item.id)
                .Select(group => new BillHistoryVM
                {
                    id = group.Key,
                    email = group.First().email,
                    address = group.First().address,
                    orderDate = group.First().orderDate,
                    price = group.Sum(item => item.price),
                    Status = group.First().Status
                }).ToList();

            result.AddRange(groupedQuery);

            return result;
        }

        public async Task<int> UpdateStatus(UpdateStatusRequest request)
        {
            var orderStatus = await _context.Orders.FindAsync(request.OrderId);



            if (orderStatus == null) throw new EShopException($"Cannot find an Order with id: {request.OrderId}");

            orderStatus.Status = request.Status;


            return await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateStockCheckout(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product with id: {productId}");
            product.Stock -= quantity;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<BillHistoryDetailVM> GetByCode(Guid code)
        {
            var result = new BillHistoryDetailVM();

            var query = await (from o in _context.Orders
                               join a in _context.AppUsers on o.UserId equals a.Id
                               join od in _context.OrderDetails on o.Id equals od.OrderId
                               join p in _context.Products on od.ProductId equals p.Id
                               join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                               where o.OrderCode == code
                               select new
                               {
                                   ID = o.Id,
                                   Name = pt.Name,
                                   Quantity = od.Quantity,
                                   Price = p.Price,
                                   Status = o.Status
                               }).ToListAsync();

            if (query.Any())
            {
                result.ID = query.First().ID;
                result.status = query.First().Status;

                // Populate the lists
                result.name = query.Select(item => item.Name).ToList();
                result.quantity = query.Select(item => item.Quantity).ToList();
                result.price = query.Select(item => item.Price).ToList();
            }

            return result;
        }
    }
}
