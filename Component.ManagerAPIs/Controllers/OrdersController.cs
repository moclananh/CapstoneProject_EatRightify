using Component.Application.Sales;
using Component.ViewModels.Sales.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.ManagerAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagerPolicy")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderService productService)
        {
            _orderService = productService;
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] OrderPagingRequest request)
        {
            var categories = await _orderService.GetAllPaging(request);
            return Ok(categories);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword)
        {
            var item = await _orderService.GetAll(keyword);
            return Ok(item);
        }

        [HttpGet("GetTotalProfit")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTotalProfit([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var item = await _orderService.TotalProfit(startDate, endDate);
            return Ok(item);
        }

        [HttpGet("GetAllByOrderStatus")]
        public async Task<IActionResult> GetAllByOrderStatus([FromQuery] GetOrderByOrderStatusRequest request)
        {
            var item = await _orderService.GetAllOrderByOrderStatus(request);
            return Ok(item);
        }

        [HttpGet("GetOrderDetailPagingRequest")]
        public async Task<IActionResult> GetOrderDetailPagingRequest([FromQuery] OrderDetailPagingRequest request)
        {
            var categories = await _orderService.GetOrderDetailPagingRequest(request);
            return Ok(categories);
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var product = await _orderService.GetById(orderId);
            if (product == null)
                return BadRequest("Cannot find Order");
            return Ok(product);
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int orderId, [FromBody] UpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.OrderId = orderId;
            try
            {
                var affectedResult = await _orderService.UpdateStatus(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpGet("GetOrderDetail/{orderId}")]
        public async Task<IActionResult> GetODById(int orderId)
        {
            var product = await _orderService.GetOrderDetail(orderId);
            if (product == null)
                return BadRequest("Cannot find Order");
            return Ok(product);
        }
    }
}
