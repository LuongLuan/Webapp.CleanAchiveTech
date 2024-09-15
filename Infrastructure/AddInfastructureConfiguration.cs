using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Infrastructure.Common;
using Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class AddInfastructureConfiguration
    {
        public static IServiceCollection AddInfastructureService(this IServiceCollection service)
        {
            service.AddScoped<IIdentityService<ApplicationUser>, IdentityService>();
            service.AddTransient(typeof(IRepositoryBaseAsync<,>),typeof(RepositoryBaseAsync<,>));
            service.AddTransient<IProductRepository, ProductService>();
            service.AddTransient<IUnitOfWork, UnitOfWork>();

            return service;
        }
    }
}
