using Component.Application.Catalog.Products;
using Component.ViewModels.Catalog.ProductImages;
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

        [HttpGet("getAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery]GetAllProductRequest request)
        {
            var products = await _productService.GetAll(request);
            return Ok(products);
        }

        [HttpGet("getProductForAI")]
        [AllowAnonymous]
        public async Task<IActionResult> getProductForAI()
        {
            var products = await _productService.GetProductForAI();
            return Ok(products);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetManageProductPagingRequest request)
        {
            var products = await _productService.GetAllPaging(request);
            return Ok(products);
        }

        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _productService.GetById(productId, languageId);
            if (product == null)
                return BadRequest("Cannot find product");
            return Ok(product);
        }

        [HttpGet("featured/{take}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedProducts(int take)
        {
            var products = await _productService.GetFeaturedProducts(take);
            return Ok(products);
        }

        [HttpGet("latest/{languageId}/{take}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatestProducts(int take, string languageId)
        {
            var products = await _productService.GetLatestProducts(languageId, take);
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
    }
}
