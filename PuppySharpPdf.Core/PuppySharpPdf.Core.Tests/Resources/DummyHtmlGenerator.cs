namespace PuppySharpPdf.Core.Tests.Resources;
public static class DummyHtmlGenerator
{
	public static string GenerateSimpleHtmlString()
	{
		var html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Resources\\html\\SimpleHtml.html");

		return html;
	}


	public static string GenerateHtmlStringWithThreeImageTags()
	{
		var html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Resources\\html\\ImageTagsHtml.html");

		return html;
	}

	public static string GenerateHtmlStringWithThreeCssTags()
	{

		var html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Resources\\html\\CssHtml.html");

		return html;
	}

	public static string GenerateHeaderHtmlString()
	{

		var html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Resources\\html\\TestHeader.html");

		return html;
	}
}
