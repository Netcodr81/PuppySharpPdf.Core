using PuppeteerSharp;
using System.Text;
using System.Text.RegularExpressions;

namespace PuppySharpPdf.Core.Utils;
internal static class HtmlUtils
{
	internal static List<string> FindImageTagSources(string html)
	{
		var matchPattern = @"<img.+?src=[\""'](.+?)[\""'].*?>";

		var matches = Regex.Matches(html, matchPattern, RegexOptions.IgnoreCase)
			.Cast<Match>()
			.Select(x => x.Groups[1].Value)
			.ToList();

		return matches;
	}

	internal static List<string> FindCssTagSources(string html)
	{
		var matchPattern = "<link[^>]*rel=[\"']stylesheet[\"'][^>]*>";

		List<string> matches = new List<string>();

		MatchCollection linkMatches = Regex.Matches(html, matchPattern);

		foreach (Match match in linkMatches)
		{
			string hrefPattern = "href=[\"']([^\"']+)[\"']";
			Match hrefMatch = Regex.Match(match.Value, hrefPattern);

			if (hrefMatch.Success)
			{
				string href = hrefMatch.Groups[1].Value;
				matches.Add(href);
			}
		}
		return matches;
	}

	internal static string RenderImageToBase64(string imgPath)
	{
		try
		{
			if (Regex.IsMatch(imgPath, @"https?://"))
			{
				using (var handler = new HttpClientHandler())
				{
					using (var client = new HttpClient(handler))
					{
						var bytes = client.GetByteArrayAsync(imgPath).Result;
						return $"data:image/{Path.GetExtension(imgPath)};base64,{Convert.ToBase64String(bytes)}";
					}
				}
			}

			var imgFileBytes = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + imgPath.Replace("~", string.Empty));
			return $"data:image/{Path.GetExtension(imgPath).Substring(1)};base64,{Convert.ToBase64String(imgFileBytes)}";
		}
		catch (Exception)
		{

			return string.Empty;
		}



	}

	internal static string NormalizeHtmlString(string html)
	{
		var imageTagsInHtml = FindImageTagSources(html);



		if (imageTagsInHtml.Count > 0)
		{
			foreach (var tag in imageTagsInHtml)
			{
				var convertedTag = RenderImageToBase64(tag);
				html = html.Replace(tag, convertedTag);
			}
		}

		return html;
	}

	internal static async Task ImportCssStyles(this IPage page, string html)
	{
		var cssPathTags = FindCssTagSources(html);

		foreach (var path in cssPathTags)
		{
			if (!Regex.IsMatch(path, @"https?://"))
			{
				var css = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}{path.Replace("~", string.Empty)}");
				await page.AddStyleTagAsync(new AddTagOptions() { Content = css });
			}

		}
	}

	internal static string NormalizeHeaderFooter(string html, List<string> styleLinks)
	{
		var styleBuilder = new StringBuilder();

		html = NormalizeHtmlString(html);

		if (styleLinks.Count > 0)
		{
			styleBuilder.Append("<style>");


			foreach (var link in styleLinks)
			{
				if (!Regex.IsMatch(link, @"https?://"))
				{
					var css = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}{link.Replace("~", string.Empty)}");
					styleBuilder.Append(css);
				}
				else
				{
					using (var handler = new HttpClientHandler())
					{
						using (var client = new HttpClient(handler))
						{
							var css = client.GetStringAsync(link).Result;
							styleBuilder.Append(css);
						}
					}
				}
			}

			styleBuilder.Append("</style>");

			styleBuilder.Append(html);

			var test = styleBuilder.ToString();

			return styleBuilder.ToString();
		}

		return html;

	}
}
