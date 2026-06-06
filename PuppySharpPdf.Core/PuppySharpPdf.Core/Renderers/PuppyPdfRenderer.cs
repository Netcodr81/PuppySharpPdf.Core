using Microsoft.Playwright;
using PuppySharpPdf.Core.Common;
using PuppySharpPdf.Core.Common.Abstractions;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Renderers.Configurations;
using System.Text.RegularExpressions;

namespace PuppySharpPdf.Core.Renderers;

public class PuppyPdfRenderer : IPuppyPdfRenderer
{
    private readonly IHtmlUtils _htmlUtils;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPlaywrightBrowserProvider _browserProvider;

    public RendererOptions RendererOptions { get; }

    public PuppyPdfRenderer(
        IHtmlUtils htmlUtils,
        IHttpClientFactory httpClientFactory,
        IPlaywrightBrowserProvider browserProvider,
        RendererOptions rendererOptions)
    {
        _htmlUtils = htmlUtils;
        _httpClientFactory = httpClientFactory;
        _browserProvider = browserProvider;
        RendererOptions = rendererOptions;
    }

    public Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url)
        => GeneratePdfFromUrlAsync(url, cancellationToken: default);

    public Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, CancellationToken cancellationToken)
        => GeneratePdfFromUrlAsync(url, new PdfOptions(), cancellationToken);

    public Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, Action<PdfOptions> pdfOptions)
        => GeneratePdfFromUrlAsync(url, pdfOptions, cancellationToken: default);

    public Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, Action<PdfOptions> pdfOptions, CancellationToken cancellationToken)
    {
        var customPdfOptions = new PdfOptions();
        pdfOptions?.Invoke(customPdfOptions);
        return GeneratePdfFromUrlAsync(url, customPdfOptions, cancellationToken);
    }

    public Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, PdfOptions pdfOptions)
        => GeneratePdfFromUrlAsync(url, pdfOptions, cancellationToken: default);

    public async Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, PdfOptions pdfOptions, CancellationToken cancellationToken)
    {
        if (url is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        cancellationToken.ThrowIfCancellationRequested();
        url = NormalizeUrl(url);

        var browser = await _browserProvider.GetBrowserAsync(cancellationToken);
        var page = await browser.NewPageAsync();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            var normalizedOptions = await NormalizeHeaderFooters(pdfOptions);
            var result = await page.PdfAsync(normalizedOptions.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PlaywrightException ex)
        {
            return Result.Failure<byte[]>(new Error("Playwright.NavigationOrRender", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<byte[]>(new Error("PdfGeneration.Unhandled", ex.Message));
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html)
        => GeneratePdfFromHtmlAsync(html, cancellationToken: default);

    public Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, CancellationToken cancellationToken)
        => GeneratePdfFromHtmlAsync(html, new PdfOptions(), cancellationToken);

    public Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, Action<PdfOptions> options)
        => GeneratePdfFromHtmlAsync(html, options, cancellationToken: default);

    public Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, Action<PdfOptions> options, CancellationToken cancellationToken)
    {
        var pdfOptions = new PdfOptions();
        options?.Invoke(pdfOptions);
        return GeneratePdfFromHtmlAsync(html, pdfOptions, cancellationToken);
    }

    public Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, PdfOptions pdfOptions)
        => GeneratePdfFromHtmlAsync(html, pdfOptions, cancellationToken: default);

    public async Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, PdfOptions pdfOptions, CancellationToken cancellationToken)
    {
        if (html is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var browser = await _browserProvider.GetBrowserAsync(cancellationToken);
        var page = await browser.NewPageAsync();

        await page.EmulateMediaAsync(new PageEmulateMediaOptions { Media = Media.Screen });

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            html = await _htmlUtils.NormalizeHtmlString(html);
            await page.SetContentAsync(html);
            await page.ImportCssStyles(html, await _htmlUtils.FindCssTagSources(html), _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient));
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var normalizedOptions = await NormalizeHeaderFooters(pdfOptions);
            var result = await page.PdfAsync(normalizedOptions.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PlaywrightException ex)
        {
            return Result.Failure<byte[]>(new Error("Playwright.HtmlRender", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<byte[]>(new Error("PdfGeneration.Unhandled", ex.Message));
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private static string NormalizeUrl(string url)
    {
        var urlValidator = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        return urlValidator.IsMatch(url) ? url : $"https://{url}";
    }

    private async Task<PdfOptions> NormalizeHeaderFooters(PdfOptions pdfOptions)
    {
        pdfOptions.HeaderTemplate = !string.IsNullOrWhiteSpace(pdfOptions.HeaderTemplate)
            ? await _htmlUtils.NormalizeHtmlString(pdfOptions.HeaderTemplate)
            : string.Empty;

        pdfOptions.FooterTemplate = !string.IsNullOrWhiteSpace(pdfOptions.FooterTemplate)
            ? await _htmlUtils.NormalizeHtmlString(pdfOptions.FooterTemplate)
            : string.Empty;

        return pdfOptions;
    }
}
