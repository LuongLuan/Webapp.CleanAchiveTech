using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ImplementInterface;
using Application.Interface;
using Domain.Common;
using Domain.Entities;
using Infrastructure.ImplementInterface;
using Microsoft.Extensions.DependencyInjection;

namespace Web.API
{
    public static class AddApplicationDependency
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection service)
        {
            service.AddTransient(typeof(IRepositoryQueryBase<,>),typeof(RepositoryQueryBase<,>));
            service.AddTransient(typeof(IRepositoryBaseAsync<,>),typeof(RepositoryBaseAsync<,>));
            service.AddTransient<IProductRepository, ProductRepository>();
            service.AddTransient<IUserRepostitory, UserRepository>();

            return service;
        }
    }
}
