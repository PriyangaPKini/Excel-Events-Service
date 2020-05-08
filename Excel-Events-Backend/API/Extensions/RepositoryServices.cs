using API.Data;
using API.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class RepositoryServices
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
        }
    }
}