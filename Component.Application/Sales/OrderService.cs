using Component.Data.EF;
using Component.Data.Entities;
using Component.Data.Enums;
using Component.Utilities.Exceptions;
using Component.ViewModels.Catalog.Products;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Bills;
using Component.ViewModels.Sales.Orders;
using Component.ViewModels.Utilities.Blogs;
using Component.ViewModels.Utilities.Promotions;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public OrderService(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<Order> Create(CheckoutRequest request)
        {
            decimal orderPrice = 0;
            decimal totalPrice = 0;
            string BuyerId = "";
            string vip = "";
            decimal originalPrice = 0;
            decimal tmp = 0;
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
                            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                            vip = user.VIP;
                            BuyerId = user.Id.ToString();
                            tmp = product.Price * orderDetailVm.Quantity;
                            totalPrice = await PriceCalculator(product.Price, orderDetailVm.Quantity, vip);
                            var orderDetail = new OrderDetail()
                            {
                                ProductId = product.Id,
                                Quantity = orderDetailVm.Quantity,
                                Price = totalPrice
                            };

                            if (orderDetail.Quantity <= product.Stock)
                            {
                                order.OrderDetails.Add(orderDetail);
                                await UpdateStockCheckout(product.Id, orderDetailVm.Quantity);
                                orderPrice += orderDetail.Price; //gia tong tien tat ca sp da qua discount
                                originalPrice += tmp;// gia tong tien tat ca sp ban dau
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
                    var test = await AccumulatedPoints(BuyerId, originalPrice);
                    transaction.Commit();
                    /*Console.WriteLine("Vip: " + vip);
                    Console.WriteLine("Gia goc: " + originalPrice.ToString());
                    Console.WriteLine("Gia co discount: " + orderPrice.ToString());
                    Console.WriteLine(BuyerId);
                    Console.WriteLine("Diem tich luy: " + test);*/
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
                            Quantity = od.Quantity,
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
                                   Id = o.Id,
                                   ShippedEmail = o.ShipEmail,
                                   UserId = a.Id,
                                   ShipName = o.ShipName,
                                   ShipPhoneNumber = o.ShipPhoneNumber,
                                   Address = o.ShipAddress,
                                   OrderDate = o.OrderDate,
                                   Status = o.Status,
                                   OrderCode = o.OrderCode
                               }).Distinct().ToListAsync();
            result.AddRange(query);

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
                                   Price = od.Price,
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
        public async Task<decimal> PriceCalculator(decimal price, int quantity, string vip)
        {
            decimal totalPrice = 0;
            decimal discount = 0;
            if (vip == null)
            {
                totalPrice = price * quantity;
            }
            if (vip == "Vip 1")
            {
                discount = price * 0.015m;
                totalPrice = (price - discount) * quantity;
            }
            if (vip == "Vip 2")
            {
                discount = price * 0.03m;
                totalPrice = (price - discount) * quantity;
            }
            if (vip == "Vip 3")
            {
                discount = price * 0.06m;
                totalPrice = (price - discount) * quantity;
            }
            if (vip == "Vip 4")
            {
                discount = price * 0.09m;
                totalPrice = (price - discount) * quantity;
            }
            if (vip == "Vip 5")
            {
                discount = price * 0.12m;
                totalPrice = (price - discount) * quantity;
            }
            return totalPrice; // gia tien da giam gia cua 1 san pham
        }

        public async Task<decimal> AccumulatedPoints(string uid, decimal price)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var userPoint = user.AccumulatedPoints;
            userPoint = price * 0.01m;
            user.AccumulatedPoints += userPoint;

            if (user.AccumulatedPoints > 0)
            {
                await Vip(uid, (int)user.AccumulatedPoints);
            }
            await _context.SaveChangesAsync();
            return (decimal)userPoint;
        }

        public async Task<int> Vip(string uid, int point)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var userPoint = point;
            if (userPoint >= 100 && userPoint < 300) // tu 100 den 299
            {
                user.VIP = "Vip 1";
            }
            if (userPoint >= 300 && userPoint < 600) // tu 300 den 599
            {
                user.VIP = "Vip 2";
            }
            if (userPoint >= 600 && userPoint < 1200) // tu 600 den 1199
            {
                user.VIP = "Vip 3";
            }
            if (userPoint >= 1200 && userPoint < 2400) // tu 1200 den 2399
            {
                user.VIP = "Vip 4";
            }
            if (userPoint >= 2400) // tu 2400 tro len
            {
                user.VIP = "Vip 5";
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<List<OrderVm>> GetAll(string keyword)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        select new {o, od, p, pt };

            //2. filter
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.o.ShipPhoneNumber.Contains(keyword));

            return await query.Select(x => new OrderVm()
            {
               Id = x.o.Id,
               OrderDate  = x.o.OrderDate,
               UserId= x.o.UserId,
               ShipName = x.o.ShipName,
               ShipAddress= x.o.ShipAddress,
               ShipEmail= x.o.ShipAddress,
               ShipPhoneNumber= x.o.ShipAddress,
               OrderCode= x.o.OrderCode,
               Status= x.o.Status,
            }).Distinct().ToListAsync();
        }

        public async Task<List<OrderDetailView>> GetOrderDetail(int id)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join l in _context.Languages on pt.LanguageId equals l.Id
                        where o.Id == id
                        select new {pt, od, pi };

            return await query.Select(x => new OrderDetailView()
            {
                ProductName = x.pt.Name,
                ImagePath = x.pi.ImagePath,
                Quantity = x.od.Quantity,
                Price = x.od.Price
            }).Distinct().ToListAsync();
        }
    }
}
