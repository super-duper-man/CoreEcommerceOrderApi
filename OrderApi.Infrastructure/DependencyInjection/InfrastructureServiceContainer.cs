using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;
using Resource.Share.Lib.DependencyInjection;

namespace OrderApi.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceContainer
    {
        public static IServiceCollection AddInfrastrctureService(this IServiceCollection services, IConfiguration config)
        {
            ShareServiceContainer.AddShareService<OrderDbContext>(services, config, config["AppSerilog:FileName"]!);

            services.AddScoped<IOrder, OrderRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicty(this IApplicationBuilder app)
        {
            ShareServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
