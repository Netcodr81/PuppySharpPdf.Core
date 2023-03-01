namespace PuppySharpPdf.Core.Renderers.Configurations;
public class PdfOptions
{
    /// <summary>
    /// Print background graphics. Defaults to <c>true</c>.
    /// </summary>
    public bool PrintBackground { get; set; } = true;

    /// <summary>
    /// Paper format. If set, takes priority over <see cref="Width"/> and <see cref="Height"/>.
    /// </summary>
    public PaperFormat Format { get; set; } = PaperFormat.A4;

    /// <summary>
    /// Scale of the webpage rendering. Defaults to <c>1</c>. Scale amount must be between 0.1 and 2.
    /// </summary>
    public decimal Scale { get; set; } = 1;

    /// <summary>
    /// Display header and footer. Defaults to <c>false</c>.
    /// </summary>
    public bool DisplayHeaderFooter { get; set; }

    /// <summary>
    /// HTML template for the print header. Should be valid HTML markup with following classes used to inject printing values into them:
    ///   <c>date</c> - formatted print date
    ///   <c>title</c> - document title
    ///   <c>url</c> - document location
    ///   <c>pageNumber</c> - current page number
    ///   <c>totalPages</c> - total pages in the document.
    /// </summary>
    public string HeaderTemplate { get; set; } = string.Empty;

    /// <summary>
    /// HTML template for the print footer. Should be valid HTML markup with following classes used to inject printing values into them:
    ///   <c>date</c> - formatted print date
    ///   <c>title</c> - document title
    ///   <c>url</c> - document location
    ///   <c>pageNumber</c> - current page number
    ///   <c>totalPages</c> - total pages in the document.
    /// </summary>
    public string FooterTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Paper orientation.. Defaults to <c>false</c>.
    /// </summary>
    public bool Landscape { get; set; }

    /// <summary>
    /// Paper ranges to print, e.g., <c>1-5, 8, 11-13</c>. Defaults to the empty string, which means print all pages.
    /// </summary>
    public string PageRanges { get; set; } = string.Empty;

    /// <summary>
    /// Paper width, accepts values labeled with units.
    /// </summary>
    public object Width { get; set; }

    /// <summary>
    /// Paper height, accepts values labeled with units.
    /// </summary>
    public object Height { get; set; }

    /// <summary>
    /// Paper margins, defaults to none.
    /// </summary>
    public MarginOptions MarginOptions { get; set; } = new MarginOptions();

    /// <summary>
    /// Give any CSS <c>@page</c> size declared in the page priority over what is declared in <c>width</c> and <c>height</c> or <c>format</c> options.
    /// Defaults to <c>false</c>, which will scale the content to fit the paper size.
    /// </summary>
    public bool PreferCSSPageSize { get; set; }

    /// <summary>
    /// Hides default white background and allows generating pdfs with transparency.
    /// </summary>
    public bool OmitBackground { get; set; }

    public PuppeteerSharp.PdfOptions MappedPdfOptions => new PuppeteerSharp.PdfOptions
    {
        PrintBackground = this.PrintBackground,
        Format = new PuppeteerSharp.Media.PaperFormat(this.Format.Width, this.Format.Height),
        Scale = this.Scale,
        DisplayHeaderFooter = this.DisplayHeaderFooter,
        FooterTemplate = this.FooterTemplate,
        HeaderTemplate = this.HeaderTemplate,
        Landscape = this.Landscape,
        PageRanges = this.PageRanges,
        Width = this.Width,
        Height = this.Height,
        MarginOptions = this.MarginOptions.MappedMarginOptions,
        PreferCSSPageSize = this.PreferCSSPageSize,
        OmitBackground = this.OmitBackground,

    };
}
