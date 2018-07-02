using Microsoft.Extensions.DependencyInjection;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Database;
using Restful.Infrastructure.Interfaces;
using Restful.Infrastructure.Repositories;

namespace Restful.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IEnhancedRepository<>), typeof(EfEnhancedRepository<>));

            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
