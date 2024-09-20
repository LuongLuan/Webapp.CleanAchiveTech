using Application.Interface;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ImplementInterface
{
    public class ProductRepository<TContext> : RepositoryBaseAsync<Product, Guid, TContext>, IProductRepository<TContext>
        where TContext : DbContext
    {
        public ProductRepository(TContext dbContext, IUnitOfWork<TContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }
    }
}
