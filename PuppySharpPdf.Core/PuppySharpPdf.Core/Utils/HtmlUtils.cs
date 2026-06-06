using PuppySharpPdf.Core.Common;
using PuppySharpPdf.Core.Interfaces;
using System.Text.RegularExpressions;

namespace PuppySharpPdf.Core.Utils;

internal class HtmlUtils : IHtmlUtils
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HtmlUtils(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Task<List<string>> FindImageTagSources(string html)
    {
        var matchPattern = @"<img.+?src=[\""'](.+?)[\""'].*?>";

        var matches = Regex.Matches(html, matchPattern, RegexOptions.IgnoreCase)
            .Cast<Match>()
            .Select(x => x.Groups[1].Value)
            .ToList();

        return Task.FromResult(matches);
    }

    public Task<List<string>> FindCssTagSources(string html)
    {
        const string matchPattern = "<link[^>]*rel=[\"']stylesheet[\"'][^>]*>";

        var matches = new List<string>();
        var linkMatches = Regex.Matches(html, matchPattern);

        foreach (Match match in linkMatches)
        {
            const string hrefPattern = "href=[\"']([^\"']+)[\"']";
            var hrefMatch = Regex.Match(match.Value, hrefPattern);

            if (hrefMatch.Success)
            {
                matches.Add(hrefMatch.Groups[1].Value);
            }
        }

        return Task.FromResult(matches);
    }

    public async Task<string> RenderImageToBase64(string imgPath)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient);
            var sourcePath = Regex.IsMatch(imgPath, @"https?://") ? imgPath : imgPath.NormalizeFilePath();

            var bytes = await client.GetByteArrayAsync(sourcePath);
            var extension = Path.GetExtension(imgPath).TrimStart('.');
            return $"data:image/{extension};base64,{Convert.ToBase64String(bytes)}";
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public async Task<string> NormalizeHtmlString(string html)
    {
        var imageTagsInHtml = await FindImageTagSources(html);

        foreach (var tag in imageTagsInHtml)
        {
            var convertedTag = await RenderImageToBase64(tag);
            html = html.Replace(tag, convertedTag);
        }

        return html;
    }
}
