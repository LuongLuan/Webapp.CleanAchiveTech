using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ImplementInterface;
using Application.Interface;
using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Web.API
{
    public static class AddApplicationDependency
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection service)
        {
            service.AddScoped(typeof(IRepositoryQueryBase<,,>),typeof(RepositoryQueryBase<,,>));
            service.AddTransient(typeof(IRepositoryBaseAsync<,,>),typeof(RepositoryBaseAsync<,,>));
            service.AddScoped(typeof(IProductRepository<>), typeof(ProductRepository<>));
            service.AddScoped(typeof(IUnitOfWork<>),typeof(UnitOfWork<>));

            return service;
        }
    }
}
