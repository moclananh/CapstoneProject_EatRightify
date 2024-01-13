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
            var affectedResult = await _orderService.UpdateStatus(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

    }
}
