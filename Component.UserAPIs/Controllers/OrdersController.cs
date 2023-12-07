﻿using Component.Application.Sales;
using Component.ViewModels.Sales.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Component.UserAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderService productService)
        {
            _orderService = productService;
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetById(id);

            if (order == null)
            {
                return NotFound(); // Return 404 Not Found if the order with the given ID is not found.
            }

            return Ok(order); // Return the order with a 200 OK status code.
        }

        [HttpGet("GetLastestOrder")]
        public async Task<IActionResult> GetLastestOrder()
        {
            var od = await _orderService.GetLastestOrderId();

            if (od == null)
            {
                return NotFound(); // Return 404 Not Found if the order with the given ID is not found.
            }

            return Ok(od); // Return the order with a 200 OK status code.
        }

        [HttpGet("GetBillHistory/{id}")]
        public async Task<IActionResult> GetBillHistory(Guid id)
        {
            var billhistory = await _orderService.BillHistory(id);
            if (billhistory == null)
                return BadRequest("Cannot find bill");
            return Ok(billhistory);
        }
        [HttpGet("GetBillById/{id}")]
        public async Task<IActionResult> GetBillById(int id)
        {
            var billhistory = await _orderService.GetBillById(id);
            if (billhistory == null)
                return BadRequest("Cannot find bill");
            return Ok(billhistory);
        }
    }
}
