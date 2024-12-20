﻿using Component.Application.Catalog.Products;
using Component.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Component.UserAPIs.Controllers
{
    //api/products
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAllProductActive")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProductActive([FromQuery] GetAllProductRequest request)
        {
            var products = await _productService.GetAllProductActive(request);
            return Ok(products);
        }

        [HttpGet("getProductForAI")]
        [AllowAnonymous]
        public async Task<IActionResult> getProductForAI()
        {
            var products = await _productService.GetProductForAI();
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetById(int productId)
        {
            var product = await _productService.GetById(productId);
            if (product == null)
                return BadRequest("Cannot find product");
            return Ok(product);
        }

        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            var products = await _productService.GetFeaturedProducts();
            return Ok(products);
        }

        [HttpPut("AddViewcount")]
        [AllowAnonymous]
        public async Task<IActionResult> AddViewcount(int productId)
        {
            try
            {
                await _productService.AddViewcount(productId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /*  [HttpGet("latest/{languageId}/{take}")]
          [AllowAnonymous]
          public async Task<IActionResult> GetLatestProducts(int take, string languageId)
          {
              var products = await _productService.GetLatestProducts(languageId, take);
              return Ok(products);
          }*/

        /*  [HttpGet("paging")]
      public async Task<IActionResult> GetAllPaging([FromQuery] GetManageProductPagingRequest request)
      {
          var products = await _productService.GetAllPaging(request);
          return Ok(products);
      }*/
    }
}
