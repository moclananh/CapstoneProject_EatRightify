using Component.Application.Sales.VNPAY_Library;
using Component.Application.Utilities.Mail;
using Component.Data.EF;
using Component.Data.Entities;
using Component.Data.Enums;
using Component.Utilities.Constants;
using Component.Utilities.Exceptions;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Bills;
using Component.ViewModels.Sales.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Component.Application.Sales
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(ApplicationDbContext context, UserManager<AppUser> userManager, IEmailService emailService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<Order> Create(CheckoutRequest request)
        {
            decimal orderPrice = 0;
            decimal totalPrice = 0;
            int vip = 0;
            decimal originalPrice = 0;
            decimal tmp = 0;
            Random generator = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randomCode = new string(Enumerable.Repeat(chars, 14)
              .Select(s => s[generator.Next(s.Length)]).ToArray());

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
                        TotalPriceOfOrder = request.TotalPriceOfOrder,
                        Status = OrderStatus.InProgress,
                        OrderCode = randomCode,
                        OrderMethod = request.OrderMethod,
                        OrderDetails = new List<OrderDetail>() { }
                    };

                    foreach (var orderDetailVm in request.OrderDetails)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDetailVm.ProductId);

                        if (product != null)
                        {
                            //var user = await _userManager.FindByIdAsync(request.UserId.ToString);
                            // vip = user.VIP;
                            // BuyerId = user.Id.ToString();
                            tmp = product.Price * orderDetailVm.Quantity;
                            //totalPrice = await PriceCalculator(product.Price, orderDetailVm.Quantity, vip);
                            var orderDetail = new OrderDetail()
                            {
                                ProductId = product.Id,
                                Quantity = orderDetailVm.Quantity,
                                Price = tmp
                            };

                            if (orderDetail.Quantity <= product.Stock)
                            {
                                order.OrderDetails.Add(orderDetail);
                                // await UpdateStockCheckout(product.Id, orderDetailVm.Quantity);  //create tam thoi k tru stock nua
                                orderPrice += orderDetail.Price; //gia tong tien tat ca sp da qua discount
                                //originalPrice += tmp;// gia tong tien tat ca sp ban dau
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
                   // var test = await AccumulatedPoints(request.UserId, orderPrice);
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
            return order;
        }

        public async Task<Order> GetLastestOrderId()
        {
            var latestOrder = await _context.Orders.OrderByDescending(order => order.Id).FirstOrDefaultAsync();
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
                            OrderMethod = o.OrderMethod,
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

        /* public async Task<int> UpdateStatus(UpdateStatusRequest request)
         {
             var orderStatus = await _context.Orders.FindAsync(request.OrderId);
             if (orderStatus == null) throw new EShopException($"Cannot find an Order with id: {request.OrderId}");
             orderStatus.Status = request.Status;
             return await _context.SaveChangesAsync();
         }*/
        public async Task<bool> UpdateStockCheckout(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product with id: {productId}");
            product.Stock -= quantity;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<CheckOrderResult<CheckOrderByCodeVm>> GetByCode(string code)
        {
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join l in _context.Languages on pt.LanguageId equals l.Id
                        where o.OrderCode == code
                        select new
                        {
                            o,
                            od,
                            p,
                            pi,
                            pt
                        };
            var orderDetails = await query.Select(x => new CheckOrderByCodeVm()
            {
                ProductId = x.p.Id,
                ProductName = x.pt.Name,
                ImagePath = x.pi.ImagePath,
                Quantity = x.od.Quantity,
                Price = x.od.Price
            }).ToListAsync();


            // Create a list to store distinct products
            List<CheckOrderByCodeVm> distinctProducts = new List<CheckOrderByCodeVm>();

            foreach (var productVm in orderDetails)
            {
                // Check if the product with the same ID is already in the distinctProducts list
                if (!distinctProducts.Any(p => p.ProductId == productVm.ProductId))
                {
                    distinctProducts.Add(productVm);
                }
            }

            var status = query.Select(x => x.o.Status).FirstOrDefault();
            var orderDate = query.Select(x => x.o.OrderDate).FirstOrDefault();
            var cancelDescription = query.Select(x => x.o.CancelDescription).FirstOrDefault();
            var refundDescription = query.Select(x => x.o.RefundDescription).FirstOrDefault();
            var totalPrice = query.Select(x => x.o.TotalPriceOfOrder).FirstOrDefault();
            // Assuming you have an enum defined for OrderStatus
            var result = new CheckOrderResult<CheckOrderByCodeVm>
            {
                Status = status,
                Items = distinctProducts,
                OrderDate = orderDate,
                CancelDescription = cancelDescription,
                RefundDescription = refundDescription,
                TotalPrice = totalPrice,
            };

            return result;
        }

        /*   public async Task<decimal> PriceCalculator(decimal price, int quantity, string vip)
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
   */
        public async Task<decimal> AccumulatedPoints(Guid uid, decimal price)
        {
            var user = await _userManager.FindByIdAsync(uid.ToString());
            if (user.UserName.Equals("guest"))
            {
                return 0;
            }
            var userPoint = price * 0.0001m;
            user.AccumulatedPoints += userPoint;

            if (user.AccumulatedPoints > 0)
            {
                await Vip(uid, (int)user.AccumulatedPoints);
            }
            await _context.SaveChangesAsync();
            return userPoint;
        }

        public async Task<int> Vip(Guid uid, int point)
        {
            var user = await _userManager.FindByIdAsync(uid.ToString());
            var userPoint = point;
            if (userPoint >= 500 && userPoint < 800)
            {
                user.VIP = 1;
            }
            if (userPoint >= 800 && userPoint < 1200) 
            {
                user.VIP = 2;
            }
            if (userPoint >= 1200 && userPoint < 1800) 
            {
                user.VIP = 3;
            }
            if (userPoint >= 1800 && userPoint < 2500) 
            {
                user.VIP = 4;
            }
            if (userPoint >= 2500) 
            {
                user.VIP = 5;
            }
            if (userPoint < 500)
            {
                user.VIP = 0;
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
                        select new { o, od, p, pt };

            //2. filter
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.o.OrderCode.Contains(keyword));

            var result = await query.Select(x => new OrderVm()
            {
                Id = x.o.Id,
                OrderDate = x.o.OrderDate,
                UserId = x.o.UserId,
                ShipName = x.o.ShipName,
                ShipAddress = x.o.ShipAddress,
                ShipEmail = x.o.ShipEmail,
                ShipPhoneNumber = x.o.ShipPhoneNumber,
                OrderCode = x.o.OrderCode,
                Status = x.o.Status,
                TotalPriceOfOrder = x.o.TotalPriceOfOrder,
                OrderMethod = x.o.OrderMethod,
            }).Distinct().ToListAsync();
            result = result.OrderByDescending(x => x.OrderDate).ToList();
            return result;
        }

        public async Task<CheckOrderResult<OrderDetailView>> GetOrderDetail(int id)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join l in _context.Languages on pt.LanguageId equals l.Id
                        where o.Id == id
                        select new { o, pt, od, pi };

            var item = await query.Select(x => new OrderDetailView()
            {
                ProductId = x.pt.ProductId,
                ProductName = x.pt.Name,
                ImagePath = x.pi.ImagePath,
                Quantity = x.od.Quantity,
                Price = x.od.Price
            }).Distinct().ToListAsync();

            var status = query.Select(x => x.o.Status).FirstOrDefault();
            var orderDate = query.Select(x => x.o.OrderDate).FirstOrDefault();
            var cancelDescription = query.Select(x => x.o.CancelDescription).FirstOrDefault();
            var refundDescription = query.Select(x => x.o.RefundDescription).FirstOrDefault();
            var totalPrice = query.Select(x => x.o.TotalPriceOfOrder).FirstOrDefault();
            var result = new CheckOrderResult<OrderDetailView>
            {
                Status = status,
                Items = item,
                OrderDate = orderDate,
                RefundDescription = refundDescription,
                CancelDescription = cancelDescription,
                TotalPrice = totalPrice,
            };

            return result;
        }

        public async Task<List<OrderVm>> GetAllOrderByOrderStatus(GetOrderByOrderStatusRequest request)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        select new { o, od, p, pt };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.o.OrderCode.Contains(request.Keyword));

            //3. filter by order status
            if (request.Status != null)
            {
                query = query.Where(x => x.o.Status == request.Status);
            }

            var orders = await query.Select(x => new OrderVm()
            {
                Id = x.o.Id,
                OrderDate = x.o.OrderDate,
                UserId = x.o.UserId,
                ShipName = x.o.ShipName,
                ShipAddress = x.o.ShipAddress,
                ShipEmail = x.o.ShipEmail,
                ShipPhoneNumber = x.o.ShipPhoneNumber,
                OrderCode = x.o.OrderCode,
                Status = x.o.Status,
                TotalPriceOfOrder = x.o.TotalPriceOfOrder,
                OrderMethod = x.o.OrderMethod,
            }).Distinct().ToListAsync();
            // Sort the users by CreatedDate after projection
            orders = orders.OrderByDescending(x => x.OrderDate).ToList();
            return orders;
        }

        public async Task<List<OrderVm>> GetUserOrderHistoryByOrderCode(GetUserOrderHistoryByOrderStatusRequest request)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        where o.UserId == request.UserId
                        select new { o, od, p, pt };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.o.OrderCode.Contains(request.Keyword));

            //3. filter by order status
            if (request.Status != null)
            {
                query = query.Where(x => x.o.Status == request.Status);
            }

            var orders = await query.Select(x => new OrderVm()
            {
                Id = x.o.Id,
                OrderDate = x.o.OrderDate,
                UserId = x.o.UserId,
                ShipName = x.o.ShipName,
                ShipAddress = x.o.ShipAddress,
                ShipEmail = x.o.ShipEmail,
                ShipPhoneNumber = x.o.ShipPhoneNumber,
                OrderCode = x.o.OrderCode,
                Status = x.o.Status,
                TotalPriceOfOrder = x.o.TotalPriceOfOrder,
                OrderMethod = x.o.OrderMethod,
            }).Distinct().ToListAsync();
            // Sort the users by CreatedDate after projection
            orders = orders.OrderByDescending(x => x.OrderDate).ToList();
            return orders;
        }

        public async Task<decimal> TotalProfit(DateTime? startDate, DateTime? endDate)
        {
            //1. Select join
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId
                        join p in _context.Products on od.ProductId equals p.Id
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        where o.Status == OrderStatus.Success
                        select new { o, od, p, pt };

            //2. filter
            if (startDate != null && endDate != null)
            {
                query = query.Where(x => x.o.OrderDate >= startDate && x.o.OrderDate <= endDate.Value.AddDays(1));
            }

            //3. Compute total price and total cost
            decimal totalPrice = query.Sum(x => x.od.Price);
            decimal totalCost = query.Sum(x => x.p.Cost * x.od.Quantity);

            //4. Calculate profit
            decimal totalProfit = totalPrice - totalCost;
            return totalProfit;
        }

        public async Task<int> CancelOrderRequest(CancelOrderRequest request)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null) throw new EShopException($"Cannot find an Order with id: {request.OrderId}");

            order.Status = OrderStatus.Canceled;
            order.CancelDescription = request.CancelDescription;
            return await _context.SaveChangesAsync();
        }

        public async Task<ApiResult<string>> InvoiceOrder(InvoiceOrderRequest request)
        {
            var latestOrder = await _context.Orders
              .Where(o => o.Id == request.OrderId)
              .OrderByDescending(o => o.OrderDate)
              .FirstOrDefaultAsync();
            var subject = "[ERS] - Thank you for shopping";
            var body = $@"
                        <html>
                        <head>
                            <style>
                                /* CSS styles for better email presentation */
                                body {{
                                    font-family: Arial, sans-serif;
                                    font-size: 14px;
                                    line-height: 1.6;
                                }}
                                .container {{
                                    max-width: 600px;
                                    margin: auto;
                                    padding: 20px;
                                    border: 1px solid #ddd;
                                    border-radius: 5px;
                                }}
                                .header {{
                                    background-color: #f4f4f4;
                                    padding: 10px;
                                    text-align: center;
                                    border-radius: 5px 5px 0 0;
                                }}
                                .content {{
                                    padding: 20px 0;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h2>{subject}</h2>
                                </div>
                                <div class='content'>
                                    <p> Thank you for choosing our system.
                                        Your order is pending, we will contact you via the phone number you provided to confirm the order in the next 6 hours, 
                                        please check your phone regularly. </p>
                                    <p>This is your Order Code: <strong>{latestOrder.OrderCode}</strong></p>
                                    <p>You can use it to check the order status <a href='https://eatrightify-system.vercel.app/customerPage/check-order'>in here</a>.</p>
                                </div>
                            </div>
                        </body>
                        </html>
                    ";

            try
            {
                await _emailService.SendEmailAsync(request.Email, subject, body);
                return new ApiSuccessMessage<string>("Email was sended");
            }
            catch
            {
                return new ApiErrorResult<string>("Error sending email");
            }
        }

        public async Task<int> RefundOrderRequest(CancelOrderRequest request)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            order.Status = OrderStatus.Refunded;
            order.RefundDescription = request.CancelDescription; // refund description
            order.RefundDate = DateTime.Now;
            return await _context.SaveChangesAsync();
        }

        public async Task<ApiResult<string>> ConfirmOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            order.Status = OrderStatus.Confirmed;
            var orderDetail = await GetOrderDetail(orderId);
            foreach (var item in orderDetail.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product.Stock < item.Quantity)
                {
                    return new ApiErrorResult<string>("Order confirm failed!");
                }
                else
                {
                    await UpdateStockCheckout(product.Id, item.Quantity);
                }
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessMessage<string>("Order confimed!");
        }

        public async Task<int> OrderSuccess(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            order.Status = OrderStatus.Success;
            order.ReceivedDate = DateTime.Now;
            return await _context.SaveChangesAsync();
        }
        public async Task<int> OrderShipping(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            order.Status = OrderStatus.Shipping;
            return await _context.SaveChangesAsync();
        }

        public async Task<string> CreateVNPayPaymentUrlAsync(VNPayRequest request)
        {
            string vnpaymentUrl = _configuration["Vnpay:PaymentUrl"];
            string vnp_TmnCode = _configuration["Vnpay:TmnCode"];
            string vnp_HashSecret = _configuration["Vnpay:HashSecret"];
            string vnp_ReturnURL = _configuration["Vnpay:ReturnUrl"];
            DateTime date = DateTime.Now;
            long orderId = DateTime.Now.Ticks;
            string ipAddress = Utils.GetIpAddress(_httpContextAccessor.HttpContext);
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", date.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", ipAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other            
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnURL);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());
            string paymentUrl = vnpay.CreateRequestUrl(vnpaymentUrl, vnp_HashSecret);
            return paymentUrl;
        }

        public async Task<decimal> ReloadAccumylatedPoint(Guid uid, decimal price)
        {
            var user = await _userManager.FindByIdAsync(uid.ToString());
            if (user.UserName.Equals("guest"))
            {
                return 0;
            }
            var priceReload = price * 0.0001m;
            user.AccumulatedPoints -= priceReload;
            await _context.SaveChangesAsync();
            return priceReload;
        }
    }
}
