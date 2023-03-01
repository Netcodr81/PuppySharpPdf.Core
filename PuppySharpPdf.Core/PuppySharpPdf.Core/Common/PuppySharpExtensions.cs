using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace PuppySharpPdf.Core.Common;
public static class PuppySharpExtensions
{
    public static async Task ImportCssStyles(this PuppeteerSharp.IPage page, string html, List<string> cssPathTags, HttpClient httpClient)
    {

        foreach (var path in cssPathTags)
        {
            if (!Regex.IsMatch(path, @"https?://"))
            {
                var css = await httpClient.GetStringAsync(path.NormalizeFilePath());
                await page.AddStyleTagAsync(new AddTagOptions() { Content = css });
            }

        }
    }

    public static string NormalizeFilePath(this string filePath)
    {
        return filePath.Replace("../", string.Empty).Replace("..//", string.Empty).Replace("wwwroot", string.Empty).Replace("./", string.Empty).Replace(".//", string.Empty).TrimStart('/').TrimStart('/');
    }
}
