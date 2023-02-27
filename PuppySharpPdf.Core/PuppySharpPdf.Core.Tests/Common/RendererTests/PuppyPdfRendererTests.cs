using PuppySharpPdf.Core.Renderers;
using PuppySharpPdf.Core.Tests.Resources;

namespace PuppySharpPdf.Core.Tests.Common.RendererTests;
public class PuppyPdfRendererTests
{
	[Fact]
	public void PuppyPdfRenderer_DefaultConstructorCalled_ShouldReturnDefaultRenderingOptions()
	{

		// arrange and act
		var renderer = new PuppyPdfRenderer();

		// assert
		renderer.RendererOptions.Headless.ShouldBe(true);
		renderer.RendererOptions.ChromeExecutablePath.ShouldBe(null);
		renderer.RendererOptions.Timeout.ShouldBe(30000);
		renderer.RendererOptions.Args.ShouldBe(Array.Empty<string>());
	}

	[Fact]
	public void PuppyPdfRenderer_CustomOptionsConstructorCalled_ShouldReturnCustomRenderingOptions()
	{

		// arrange and act
		var renderer = new PuppyPdfRenderer(options =>
		{
			options.Headless = false;
			options.IgnoreHTTPSErrors = true;
			options.ChromeExecutablePath = "C:\\Test\\chrome.exe";
			options.Timeout = 500;
		});

		// assert
		renderer.RendererOptions.Headless.ShouldBe(false);
		renderer.RendererOptions.ChromeExecutablePath.ShouldBe("C:\\Test\\chrome.exe");
		renderer.RendererOptions.Timeout.ShouldBe(500);
		renderer.RendererOptions.Args.ShouldBe(Array.Empty<string>());
	}

	[Fact]
	public async Task GeneratePdfFromUrlAsync_DefaultRendererWithProtocol_ShouldReturnNonEmptyByteArray()
	{

		// arrange
		var renderer = new PuppyPdfRenderer();

		// act
		var result = await renderer.GeneratePdfFromUrlAsync("https://www.google.com");

		// assert
		result.Value.ShouldNotBeEmpty();
	}


	[Fact]
	public async Task GeneratePdfFromUrlAsync_DefaultRendererWithNoProtocol_ShouldReturnNonEmptyByteArray()
	{

		// arrange
		var renderer = new PuppyPdfRenderer();

		// act
		var result = await renderer.GeneratePdfFromUrlAsync("www.google.com");

		// assert
		result.Value.ShouldNotBeEmpty();
	}

	[Fact]
	public async Task GeneratePdfFromUrlAsync_CustomPdfOptions_ShouldReturnNonEmptyByteArray()
	{

		// arrange
		var renderer = new PuppyPdfRenderer();

		// act
		var result = await renderer.GeneratePdfFromUrlAsync("www.google.com", options =>
		{
			options.Format = PuppySharpPdf.Core.Renderers.Configurations.PaperFormat.Letter;

		});

		// assert
		result.Value.ShouldNotBeEmpty();
	}

	[Fact]
	public async Task GeneratePdfFromHtml_NoCustomOptions_ShouldReturnNonEmptyByteArray()
	{

		// arrange
		var html = DummyHtmlGenerator.GenerateSimpleHtmlString();
		var renderer = new PuppyPdfRenderer();

		// act
		var result = await renderer.GeneratePdfFromHtmlAsync(html);

		// assert

		result.Value.ShouldNotBeEmpty();

	}
}
