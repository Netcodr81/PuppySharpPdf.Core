using Microsoft.Extensions.DependencyInjection;
using PuppySharpPdf.Core.Common;
using PuppySharpPdf.Core.Common.Mapping;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Utils;

namespace PuppySharpPdf.Core.Renderers.Configurations;
public static class PuppySharpConfiguration
{
    public static IServiceCollection AddPuppySharpPdfCore(this IServiceCollection services, Action<HttpClient> httpClientConfig)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (httpClientConfig == null) throw new ArgumentNullException(nameof(httpClientConfig));



        services.AddHttpClient(ConfigConstants.PuppyHttpClient, client => httpClientConfig.Invoke(client));
        services.AddScoped<IHtmlUtils, HtmlUtils>();
        services.AddScoped<IPuppyPdfRenderer, PuppyPdfRenderer>();
        services.AddScoped<IPuppyMapper, PuppyMapper>();

        return services;
    }

    public static IServiceCollection AddPuppySharpPdfCore(this IServiceCollection services, Action<RendererOptions> renderOptions, Action<HttpClient> httpClientConfig)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        if (httpClientConfig == null) throw new ArgumentNullException(nameof(httpClientConfig));



        services.AddHttpClient(ConfigConstants.PuppyHttpClient, client => httpClientConfig.Invoke(client));
        services.AddScoped<IHtmlUtils, HtmlUtils>();
        services.AddScoped<IPuppyMapper, PuppyMapper>();
        services.AddScoped<IPuppyPdfRenderer, PuppyPdfRenderer>(options =>
        {
            return new PuppyPdfRenderer(options => renderOptions.Invoke(options), options.GetService<IHtmlUtils>(), options.GetService<IPuppyMapper>(), options.GetService<IHttpClientFactory>());
        });




        return services;
    }
}
