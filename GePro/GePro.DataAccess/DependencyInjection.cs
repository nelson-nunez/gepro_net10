using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GePro.DataAccess
{
    public static class DependencyInjection
    {
        /// <remarks>La capa API debe registrar además una implementación de <see cref="ICurrentUser"/>.</remarks>
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No hay string de conexión...");

            services.AddDbContext<DbModelContext>(options => options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies());

            services.TryAddSingleton(TimeProvider.System);
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
