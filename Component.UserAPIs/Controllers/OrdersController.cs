using Component.Application.Catalog.Products;
using Component.Application.Sales;
using Component.ViewModels.Common;
using Component.ViewModels.Sales.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        public OrdersController(
            IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public class CustomErrorDetails : ProblemDetails
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _orderService.Create(request);
            if (result == null)
            {
                var errorDetails = new CustomErrorDetails
                {
                    Type = "Error",
                    Title = "Cannot purchase greater than product quantity",
                    Status = 400,
                    Detail = "One or more products in the shopping cart exceed the number of products in stock",
                    Instance = HttpContext.Request.Path
                };

                return BadRequest(errorDetails);
            }
            return Ok(request);
        }


        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetById(id);

            if (order == null)
            {
                return NoContent(); // Return 404 Not Found if the order with the given ID is not found.
            }

            return Ok(order); // Return the order with a 200 OK status code.
        }


        [HttpGet("GetLastestOrder")]
        public async Task<IActionResult> GetLastestOrder()
        {
            var od = await _orderService.GetLastestOrderId();

            if (od == null)
            {
                return NoContent(); // Return 404 Not Found if the order with the given ID is not found.
            }

            return Ok(od); // Return the order with a 200 OK status code.
        }

        [HttpGet("GetBillHistory/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetBillHistory(Guid userId)
        {
            var billhistory = await _orderService.BillHistory(userId);
            if (billhistory == null)
                return BadRequest("Cannot find bill");
            return Ok(billhistory);
        }

        [HttpGet("GetBillDetails/{id}")]
        [Authorize]
        public async Task<IActionResult> GetBillDetails(int id)
        {
            var item = await _orderService.GetOrderDetail(id);
            return Ok(item);
        }


        [HttpGet("GetByOrderCode/{orderCode}")]
        public async Task<IActionResult> GetOrderbyCode(string orderCode)
        {
            var order = await _orderService.GetByCode(orderCode);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("GetUserOrderHistoryByOrderStatus")]
        public async Task<IActionResult> GetUserOrderHistoryByOrderStatus([FromQuery] GetUserOrderHistoryByOrderStatusRequest request)
        {
            var item = await _orderService.GetUserOrderHistoryByOrderCode(request);
            return Ok(item);
        }

        [HttpPut("RefundOrder")]
        [AllowAnonymous]
        public async Task<IActionResult> RefundOrder(CancelOrderRequest request)
        {
            var order = await _orderService.GetById(request.OrderId);
            if (order.ReceivedDate == null)
            {
                return BadRequest("The order has not been delivered to the user");
            }
            DateTime tmp = DateTime.Now;// time want refund
            TimeSpan timeDifference = (TimeSpan)(tmp - order.ReceivedDate);
            if (timeDifference.TotalDays > 7)
            {
                return BadRequest("Cannot refund after 7 day from the recieved date");
            }
            if (order.Status == Data.Enums.OrderStatus.Success)
            {
                await _orderService.RefundOrderRequest(request);
                var orderDetail = await _orderService.GetOrderDetail(request.OrderId);
                // xu ly re-stock
                foreach (var item in orderDetail.Items)
                {
                    // Gọi hàm ReStock để cập nhật lại tồn kho cho từng sản phẩm
                    await _productService.ReStock(item.ProductId, item.Quantity);
                }
                return Ok();
            }
            return BadRequest("Only refund order when user recieved order!");
        }


        [HttpPut("CancelOrderRequest")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelOrderRequest(CancelOrderRequest request)
        {
            var order = await _orderService.GetById(request.OrderId);
            if (order.Status == Data.Enums.OrderStatus.InProgress)
            {
                await _orderService.CancelOrderRequest(request);
                return Ok();
            }
            return BadRequest("Cannot Cancel Order");
        }


        [HttpPost("InvoiceOrder")]
        [AllowAnonymous]
        public async Task<IActionResult> InvoiceOrder([FromBody]InvoiceOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.InvoiceOrder(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("VNPay")]
        public async Task<IActionResult> VNPay([FromBody] VNPayRequest request)
        {
            try
            {
                var paymentUrl = await _orderService.CreateVNPayPaymentUrlAsync(request);
                return Ok(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create VNPay payment URL: {ex.Message}");
            }
        }
    }
} 
