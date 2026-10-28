using Kcl.Reso.CleanTemplate.Web.Api.Middleware;

namespace Kcl.Reso.CleanTemplate.Web.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();

        return app;
    }
}
