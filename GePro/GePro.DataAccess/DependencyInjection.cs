using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GePro.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No hay string de conexión...");

            services.AddDbContext<DbModelContext>(options => options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies());

            return services;
        }
    }
}
