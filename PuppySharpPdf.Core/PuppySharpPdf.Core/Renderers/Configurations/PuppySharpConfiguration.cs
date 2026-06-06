using Microsoft.Extensions.DependencyInjection;
using PuppySharpPdf.Core.Common;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Renderers;
using PuppySharpPdf.Core.Utils;

namespace PuppySharpPdf.Core.Renderers.Configurations;

public static class PuppySharpConfiguration
{
    public static IServiceCollection AddPuppySharpPdfCore(this IServiceCollection services, Action<HttpClient> httpClientConfig)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (httpClientConfig is null) throw new ArgumentNullException(nameof(httpClientConfig));

        var rendererOptions = new RendererOptions();
        return AddPuppySharpPdfCore(services, rendererOptions, httpClientConfig);
    }

    public static IServiceCollection AddPuppySharpPdfCore(this IServiceCollection services, Action<RendererOptions> renderOptions, Action<HttpClient> httpClientConfig)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (renderOptions is null) throw new ArgumentNullException(nameof(renderOptions));
        if (httpClientConfig is null) throw new ArgumentNullException(nameof(httpClientConfig));

        var rendererOptions = new RendererOptions();
        renderOptions(rendererOptions);

        return AddPuppySharpPdfCore(services, rendererOptions, httpClientConfig);
    }

    private static IServiceCollection AddPuppySharpPdfCore(IServiceCollection services, RendererOptions rendererOptions, Action<HttpClient> httpClientConfig)
    {
        services.AddHttpClient(ConfigConstants.PuppyHttpClient, httpClientConfig);

        services.AddSingleton(rendererOptions);
        services.AddSingleton<IPlaywrightBrowserProvider, PlaywrightBrowserProvider>();

        services.AddScoped<IHtmlUtils, HtmlUtils>();
        services.AddScoped<IPuppyPdfRenderer, PuppyPdfRenderer>();

        return services;
    }
}
