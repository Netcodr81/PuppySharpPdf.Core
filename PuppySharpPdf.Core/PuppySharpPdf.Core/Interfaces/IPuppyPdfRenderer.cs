using PuppySharpPdf.Core.Common.Abstractions;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Interfaces;
public interface IPuppyPdfRenderer
{
    Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html);
    Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, CancellationToken cancellationToken);
    Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, Action<PdfOptions> options);
    Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, Action<PdfOptions> options, CancellationToken cancellationToken);
    Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, PdfOptions pdfOptions);
    Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, PdfOptions pdfOptions, CancellationToken cancellationToken);
    Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url);
    Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, CancellationToken cancellationToken);
    Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, Action<PdfOptions> pdfOptions);
    Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, Action<PdfOptions> pdfOptions, CancellationToken cancellationToken);
    Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, PdfOptions pdfOptions);
    Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, PdfOptions pdfOptions, CancellationToken cancellationToken);

    RendererOptions RendererOptions { get; }
}
