using PuppeteerSharp;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Interfaces;
public interface IPuppyMapper
{
    LaunchOptions MapToLaunchOptions(RendererOptions options);
}
