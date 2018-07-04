using Microsoft.Extensions.DependencyInjection;
using Restful.Infrastructure.Resources.PropertyMappings;
using Restful.Infrastructure.Services;

namespace Restful.Infrastructure.Extensions
{
    public static class PropertyMappingExtensions
    {
        public static void AddPropertyMappings(this IServiceCollection services)
        {
            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<CountryPropertyMapping>();

            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);
            services.AddTransient<ITypeHelperService, TypeHelperService>();
        }
    }
}
