
using Infrastructure.ImplementInterface;
using Infrastructure.Interface;

namespace Web.API
{
    public static class AddInfastructureDependency
    {
        public static IServiceCollection AddInfastructureService(this IServiceCollection service)
        {
            service.AddScoped<IUserService, UserService>();
            service.AddTransient<IProductService, ProductService>();
            return service;
        }
    }
}
