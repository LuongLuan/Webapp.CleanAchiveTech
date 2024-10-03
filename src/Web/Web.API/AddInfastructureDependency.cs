
using Application.ImplementInterface;
using Application.Interface;
using Infrastructure.ImplementInterface;

namespace Web.API
{
    public static class AddInfastructureDependency
    {
        public static IServiceCollection AddInfastructureService(this IServiceCollection service)
        {
            service.AddTransient<IUserService, UserService>();
            service.AddTransient<IProductService, ProductService>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            return service;
        }
    }
}
