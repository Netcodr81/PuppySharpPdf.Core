namespace PuppySharpPdf.Core.Interfaces;
public interface IHtmlUtils
{
    Task<List<string>> FindImageTagSources(string html);
    Task<List<string>> FindCssTagSources(string html);
    Task<string> RenderImageToBase64(string imgPath);
    Task<string> NormalizeHtmlString(string html);
}
