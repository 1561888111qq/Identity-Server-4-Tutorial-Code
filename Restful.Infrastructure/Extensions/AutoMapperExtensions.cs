using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Restful.Infrastructure.Extensions
{
    public static class AutoMapperExtensions
    {
        public static void AddAutoMapperAndMappings(this IServiceCollection services)
        {
            services.AddAutoMapper();
        }
    }
}
