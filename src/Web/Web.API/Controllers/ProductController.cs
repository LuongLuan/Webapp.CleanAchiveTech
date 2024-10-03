using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.API.Constants;
using Web.API.CustomAtrribute;
using Web.API.DTO;

namespace Web.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize]
        [CustomPolicy(ConstantAPI.VIEW_PRODUCT)]
        public async Task<IActionResult> GetProduct(string productId)
        {
            // 6E8EFBB5-E59E-4C41-82A7-38AFC8F4EF4E
            var result = await _productService.GetProductByIdAsync(new Guid(productId));
            return new JsonResult(new APIResultDto
            {
                Code = HttpStatusCode.OK,
                Data = result
            });
        }
    }
}
