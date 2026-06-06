using Microsoft.Playwright;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Interfaces;

public interface IPuppyMapper
{
    BrowserTypeLaunchOptions MapToLaunchOptions(RendererOptions options);
}
