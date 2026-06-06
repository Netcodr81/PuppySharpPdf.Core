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
    private readonly IPuppyMapper _puppyMapper;
    private readonly IHttpClientFactory _httpClientFactory;

    public RendererOptions RendererOptions { get; }

    public PuppyPdfRenderer(IHtmlUtils htmlUtils, IPuppyMapper puppyMapper, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _puppyMapper = puppyMapper;
        _htmlUtils = htmlUtils;
        RendererOptions = new RendererOptions();
    }

    public PuppyPdfRenderer(Action<RendererOptions> options, IHtmlUtils htmlUtils, IPuppyMapper puppyMapper, IHttpClientFactory httpClientFactory)
    {
        _htmlUtils = htmlUtils;
        _httpClientFactory = httpClientFactory;
        _puppyMapper = puppyMapper;

        var rendererOptions = new RendererOptions();
        options?.Invoke(rendererOptions);
        RendererOptions = rendererOptions;
    }

    public async Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url)
    {
        if (url is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        url = NormalizeUrl(url);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(_puppyMapper.MapToLaunchOptions(RendererOptions));
        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            var result = await page.PdfAsync();
            return Result.Success(result);
        }
        catch (Exception)
        {
            return Result.Failure<byte[]>(new Error("500", "An error occurred while generating the pdf"));
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, Action<PdfOptions> pdfOptions)
    {
        if (url is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        url = NormalizeUrl(url);

        var customPdfOptions = new PdfOptions();
        pdfOptions?.Invoke(customPdfOptions);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(_puppyMapper.MapToLaunchOptions(RendererOptions));
        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            var normalizedOptions = await NormailzeHeaderFooters(customPdfOptions);
            var result = await page.PdfAsync(normalizedOptions.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception)
        {
            return Result.Failure<byte[]>(new Error("500", "An error occurred while generating the pdf"));
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, PdfOptions pdfOptions)
    {
        if (url is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        url = NormalizeUrl(url);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(_puppyMapper.MapToLaunchOptions(RendererOptions));
        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            var normalizedOptions = await NormailzeHeaderFooters(pdfOptions);
            var result = await page.PdfAsync(normalizedOptions.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception)
        {
            return Result.Failure<byte[]>(new Error("500", "An error occurred while generating the pdf"));
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html)
    {
        if (html is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(_puppyMapper.MapToLaunchOptions(RendererOptions));
        var page = await browser.NewPageAsync();

        await page.EmulateMediaAsync(new PageEmulateMediaOptions { Media = Media.Screen });

        try
        {
            html = await _htmlUtils.NormalizeHtmlString(html);
            await page.SetContentAsync(html);
            await page.ImportCssStyles(html, await _htmlUtils.FindCssTagSources(html), _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient));
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var result = await page.PdfAsync(new PdfOptions().MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception)
        {
            return Result.Failure<byte[]>(new Error("500", "An error occurred while generating the pdf"));
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, Action<PdfOptions> options)
    {
        if (html is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        var pdfOptions = new PdfOptions();
        options?.Invoke(pdfOptions);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(_puppyMapper.MapToLaunchOptions(RendererOptions));
        var page = await browser.NewPageAsync();

        await page.EmulateMediaAsync(new PageEmulateMediaOptions { Media = Media.Screen });

        try
        {
            html = await _htmlUtils.NormalizeHtmlString(html);
            await page.SetContentAsync(html);
            await page.ImportCssStyles(html, await _htmlUtils.FindCssTagSources(html), _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient));
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var normalizedOptions = await NormailzeHeaderFooters(pdfOptions);
            var result = await page.PdfAsync(normalizedOptions.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception)
        {
            return Result.Failure<byte[]>(new Error("500", "An error occurred while generating the pdf"));
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, PdfOptions pdfOptions)
    {
        if (html is null)
        {
            return Result.Failure<byte[]>(Error.EmptyUrl);
        }

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(_puppyMapper.MapToLaunchOptions(RendererOptions));
        var page = await browser.NewPageAsync();

        await page.EmulateMediaAsync(new PageEmulateMediaOptions { Media = Media.Screen });

        try
        {
            html = await _htmlUtils.NormalizeHtmlString(html);
            await page.SetContentAsync(html);
            await page.ImportCssStyles(html, await _htmlUtils.FindCssTagSources(html), _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient));
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var normalizedOptions = await NormailzeHeaderFooters(pdfOptions);
            var result = await page.PdfAsync(normalizedOptions.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception)
        {
            return Result.Failure<byte[]>(new Error("500", "An error occurred while generating the pdf"));
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

    private async Task<PdfOptions> NormailzeHeaderFooters(PdfOptions pdfOptions)
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

