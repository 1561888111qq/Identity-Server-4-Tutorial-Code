using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Restful.Infrastructure.Resources;
using Restful.Infrastructure.Resources.Validators;

namespace Restful.Infrastructure.Extensions
{
    public static class FluetValidationExtensions
    {
        public static void AddFluetValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<CityAddResource>, CityAddOrUpdateResourceValidator<CityAddResource>>();
            services.AddTransient<IValidator<CityUpdateResource>, CityUpdateResourceValidator>();
            services.AddTransient<IValidator<CountryAddResource>, CountryAddResourceValidator>();
        }
    }
}
