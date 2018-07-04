using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Restful.Api.Helpers
{
    public static class ExceptionHandlingMiddleware
    {
        public static void UseMyExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.UseCors("AllowAngularDevOrigin");
                builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global Exception Logger");
                            logger.LogError(500, ex.Error, ex.Error.Message);
                        }
                        await context.Response.WriteAsync(ex?.Error?.Message ?? "An Error Occurred.");
                    });
            });
        }
    }
}
