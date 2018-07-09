using System.Collections.Generic;
using System.IO;
using System.Text;
using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Restful.Api.Authorization;
using Restful.Api.Helpers;
using Restful.Infrastructure.Database;
using Restful.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;

namespace Restful.Api
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _loggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.ApiName = "restapi";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanGetCountries", builder =>
                    {
                        builder.RequireAuthenticatedUser();
                        builder.AddRequirements(new MustRequirement());
                    });
            });

            services.AddSingleton<IAuthorizationHandler, MustHandler>();

            services.AddMvc(options =>
                {
                    options.ReturnHttpNotAcceptable = true;
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<MyContext>());

            services.AddMediaTypes();

            services.AddDbContext<MyContext>(options =>
            {
                // options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseInMemoryDatabase("RESTful");
                options.UseLoggerFactory(_loggerFactory);
            });

            services.AddAutoMapperAndMappings();
            services.AddFluetValidators();

            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                    .ActionContext;
                return new UrlHelper(actionContext);
            });

            // services.AddHttpCacheHeaders(
            //     expirationModelOptions =>
            //     {
            //         expirationModelOptions.MaxAge = 300;

            //     },
            //     validationModelOptions =>
            //     {
            //         validationModelOptions.AddMustRevalidate = true;
            //     });

            // services.AddResponseCaching();

            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //         .AddJwtBearer(options =>
            //         {
            //             var serverSecret = new SymmetricSecurityKey(
            //                 Encoding.UTF8.GetBytes(Configuration["JWT:ServerSecret"]));

            //             options.TokenValidationParameters = new TokenValidationParameters
            //             {
            //                 IssuerSigningKey = serverSecret,
            //                 ValidIssuer = Configuration["JWT:Issuer"],
            //                 ValidAudience = Configuration["JWT:Audience"]
            //             };
            //         });

            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(60);
            //    options.ExcludedHosts.Add("example.com");
            //    options.ExcludedHosts.Add("www.example.com");
            //});

            // services.AddHttpsRedirection(options =>
            // {
            //     options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            //     options.HttpsPort = 5001;
            // });

            // services.AddCors(options =>
            // {
            //     options.AddPolicy("AllowAngularDevOrigin",
            //         builder => builder.WithOrigins("http://localhost:4200")
            //         .AllowAnyHeader()
            //         .AllowAnyMethod());
            // });

            services.Configure<MvcOptions>(options =>
            {
                // options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAngularDevOrigin"));

                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // services.AddMemoryCache();
            // services.Configure<IpRateLimitOptions>(options =>
            // {
            //     options.GeneralRules = new List<RateLimitRule>
            //     {
            //         new RateLimitRule
            //         {
            //             Endpoint = "*",
            //             Limit = 100,
            //             Period = "1m"
            //         }
            //     };
            // });
            // services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            // services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            services.AddPropertyMappings();
            services.AddRepositories();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMyExceptionHandler(_loggerFactory);
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }
            app.UseAuthentication();
            // app.UseIpRateLimiting();
            // app.UseCors("AllowAngularDevOrigin");
            app.UseHttpsRedirection();
            // app.UseResponseCaching();
            // app.UseHttpCacheHeaders();

            app.UseMvc();
        }
    }
}
