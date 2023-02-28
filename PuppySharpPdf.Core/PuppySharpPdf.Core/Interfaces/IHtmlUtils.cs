namespace PuppySharpPdf.Core.Interfaces;
public interface IHtmlUtils
{
    List<string> FindImageTagSources(string html);
    List<string> FindCssTagSources(string html);
    string RenderImageToBase64(string imgPath);
    string NormalizeHtmlString(string html);
    string NormalizeHeaderFooter(string html, List<string> styleLinks);
}
