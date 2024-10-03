using Application.Interface;
using Domain.Entities;
using Domain.Persistence;

namespace Infrastructure.ImplementInterface
{
    public class ProductRepository: RepositoryBaseAsync<Product, Guid>, IProductRepository
    {
        public ProductRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}
