using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Restful.Infrastructure.Database;

namespace Restful.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Restful API Demo";
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var salesContext = services.GetRequiredService<MyContext>();
                    MyContextSeed.SeedAsync(salesContext, loggerFactory).Wait();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseUrls("http://localhost:6000", "https://localhost:6001")
                .UseStartup<Startup>();
    }
}
