using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IProductRepository<TContext> : IRepositoryBaseAsync<Product, Guid, TContext>
        where TContext : DbContext
    {
    }
}
