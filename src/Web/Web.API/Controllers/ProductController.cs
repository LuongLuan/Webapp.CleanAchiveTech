using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Web.API.Constants;
using Web.API.CustomAtrribute;
using Web.API.Model;

namespace Web.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize]
        [CustomPolicy("bbbbb")]
        public async Task<IActionResult> GetProduct(string productId)
        {
            // 6E8EFBB5-E59E-4C41-82A7-38AFC8F4EF4E
            var result = await _repository.GetByIdAsync(new Guid(productId));
            return new JsonResult(new APIResult
            {
                Code = HttpStatusCode.OK,
                Data = result
            });
        }
    }
}
