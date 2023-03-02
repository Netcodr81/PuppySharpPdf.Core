using PuppeteerSharp;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Common.Mapping; internal class PuppyMapper : IPuppyMapper
{
    readonly IHtmlUtils _htmlUtils;
    public PuppyMapper(IHtmlUtils htmlUtils)
    {
        _htmlUtils = htmlUtils;
    }
    public LaunchOptions MapToLaunchOptions(RendererOptions options)
    {
        return new LaunchOptions
        {
            IgnoreHTTPSErrors = options.IgnoreHTTPSErrors,
            Headless = options.Headless,
            ExecutablePath = options.ChromeExecutablePath,
            Args = options.Args,
            Timeout = options.Timeout
        };
    }

}
