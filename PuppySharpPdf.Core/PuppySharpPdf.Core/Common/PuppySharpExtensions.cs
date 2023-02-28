using PuppeteerSharp;
using PuppySharpPdf.Core.Interfaces;
using System.Text.RegularExpressions;

namespace PuppySharpPdf.Core.Common;
public static class PuppySharpExtensions
{
    public static async Task ImportCssStyles(this PuppeteerSharp.IPage page, string html, List<string> cssPathTags)
    {

        foreach (var path in cssPathTags)
        {
            if (!Regex.IsMatch(path, @"https?://"))
            {
                var css = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}{path.Replace("~", string.Empty)}");
                await page.AddStyleTagAsync(new AddTagOptions() { Content = css });
            }

        }
    }

    public static string NormalizeHtmlString(string html, IHtmlUtils htmlUtils)
    {
        return htmlUtils.NormalizeHtmlString(html);
    }
}
