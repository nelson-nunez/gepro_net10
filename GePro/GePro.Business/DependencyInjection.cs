using GePro.Business.Security;
using GePro.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GePro.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services)
        {
            services.AddAutoMapper(configuration => { }, typeof(DependencyInjection).Assembly);

            services.AddSingleton<IPasswordService, PasswordService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<INeighborhoodService, NeighborhoodService>();
            services.AddScoped<IGenderService, GenderService>();

            return services;
        }
    }
}
