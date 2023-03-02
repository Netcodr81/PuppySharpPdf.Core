using PuppySharpPdf.Core.Common;
using PuppySharpPdf.Core.Interfaces;
using System.Text.RegularExpressions;

namespace PuppySharpPdf.Core.Utils;
internal class HtmlUtils : IHtmlUtils
{

    readonly IHttpClientFactory _httpClientFactory;
    public HtmlUtils(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }
    public async Task<List<string>> FindImageTagSources(string html)
    {
        var matchPattern = @"<img.+?src=[\""'](.+?)[\""'].*?>";

        var matches = Regex.Matches(html, matchPattern, RegexOptions.IgnoreCase)
            .Cast<Match>()
            .Select(x => x.Groups[1].Value)
            .ToList();

        return matches;
    }

    public async Task<List<string>> FindCssTagSources(string html)
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

    public async Task<string> RenderImageToBase64(string imgPath)
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

            var client2 = _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient);

            var imgFileBytes = await client2.GetByteArrayAsync(imgPath.NormalizeFilePath());
            return $"data:image/{Path.GetExtension(imgPath).Substring(1)};base64,{Convert.ToBase64String(imgFileBytes)}";
        }
        catch (Exception)
        {

            return string.Empty;
        }



    }

    public async Task<string> NormalizeHtmlString(string html)
    {
        var imageTagsInHtml = await FindImageTagSources(html);



        if (imageTagsInHtml.Count > 0)
        {
            foreach (var tag in imageTagsInHtml)
            {
                var convertedTag = await RenderImageToBase64(tag);
                html = html.Replace(tag, convertedTag);
            }
        }

        return html;
    }


}
