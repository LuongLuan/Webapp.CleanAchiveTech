using Application.Interface;
using Domain.Entities;

namespace Application.ImplementInterface
{
    public class ProductService:IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetProductByIdAsync(Guid productId)
        {
            var result = await _productRepository.GetByIdAsync(productId);
            return result;
        }
    }
}
