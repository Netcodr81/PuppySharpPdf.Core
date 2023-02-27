using PuppySharpPdf.Core.Common.Mapping;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Tests.Common.MappingTests;
public class MappingTests
{
	[Fact]
	public void RendererOptions_MapsToLaunchOptions()
	{

		// arrange

		var options = new RendererOptions()
		{
			Headless = false,
			ChromeExecutablePath = "C:\\test\\chrome.exe",
			Timeout = 500
		};

		// act

		var result = PuppyMapper.MapToLaunchOptions(options);

		// assert
		result.IgnoreHTTPSErrors.ShouldBe(false);
		result.Headless.ShouldBe(false);
		result.ExecutablePath.ShouldBe("C:\\test\\chrome.exe");
		result.Args.ShouldBe(Array.Empty<string>());
		result.Timeout.ShouldBe(500);
		result.Env.ShouldBe(new Dictionary<string, string>());
	}

	[Fact]
	public void PuppySharpPdfOptionsClass_AsksForGetMappedPdf_ShouldReturnPuppeteerSharpPdfOptionsClassWithLandscapeSetToTrue()
	{

		// arrange
		var pdfOptions = new PdfOptions { Landscape = true };

		// act
		var result = pdfOptions.MappedPdfOptions;

		// assert
		result.Landscape.ShouldBe(true);
		result.ShouldBeOfType<PuppeteerSharp.PdfOptions>();

	}
}
