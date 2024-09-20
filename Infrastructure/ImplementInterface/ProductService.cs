using Application.Interface;
using Domain.Common;
using Domain.Entities;
using Domain.Persistence;
using Infrastructure.Interface;

namespace Infrastructure.ImplementInterface
{
    public class ProductService:IProductService
    {
        private readonly IProductRepository<AppDbContext> _productRepository;

        public ProductService(IProductRepository<AppDbContext> productRepository)
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
