using PuppeteerSharp;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Common.Mapping;
internal class PuppyMapper
{
	internal static LaunchOptions MapToLaunchOptions(RendererOptions options)
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
