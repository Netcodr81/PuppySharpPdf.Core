using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Utils;

namespace PuppySharpPdf.Core.Renderers.Configurations;
public static class PuppySharpConfiguration
{
    public static IServiceCollection AddPuppySharpPdfCore(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddHttpContextAccessor();

        services.AddScoped<IHtmlUtils, HtmlUtils>();
        services.AddScoped<IPuppyPdfRenderer, PuppyPdfRenderer>();

        return services;
    }

    public static void UsePuppySharpPdfCore(this IApplicationBuilder app)
    {
        app.Use((context, next) =>
        {
            context.Items["HttpContext"] = context;
            return next();
        });
    }

    internal static HttpContext GetHttpContext(this HttpContext context)
    {
        return (HttpContext)context.Items["HttpContext"];
    }
}
