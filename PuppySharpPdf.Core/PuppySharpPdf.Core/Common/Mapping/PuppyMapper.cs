using Microsoft.Playwright;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Common.Mapping;

internal class PuppyMapper : IPuppyMapper
{
    readonly IHtmlUtils _htmlUtils;
    public PuppyMapper(IHtmlUtils htmlUtils)
    {
        _htmlUtils = htmlUtils;
    }
    public BrowserTypeLaunchOptions MapToLaunchOptions(RendererOptions options)
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = options.Headless,
            ExecutablePath = options.ChromeExecutablePath,
            Args = options.Args,
            Timeout = options.Timeout
        };
    }

}
