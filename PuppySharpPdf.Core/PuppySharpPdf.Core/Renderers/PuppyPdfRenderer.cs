using Ardalis.Result;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using PuppySharpPdf.Core.Common;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Renderers.Configurations;
using System.Text.RegularExpressions;


namespace PuppySharpPdf.Core.Renderers; public class PuppyPdfRenderer : IPuppyPdfRenderer
{
    readonly IHtmlUtils _htmlUtils;
    readonly IPuppyMapper _puppyMapper;
    readonly IHttpClientFactory _httpClientFactory;
    public RendererOptions RendererOptions { get; }

    public PuppyPdfRenderer(IHtmlUtils htmlUtils, IPuppyMapper puppyMapper, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _puppyMapper = puppyMapper;
        RendererOptions = new RendererOptions();
        _htmlUtils = htmlUtils;
    }

    public PuppyPdfRenderer(Action<RendererOptions> options, IHtmlUtils htmlUtils, IPuppyMapper puppyMapper, IHttpClientFactory httpClientFactory)
    {
        _htmlUtils = htmlUtils;
        _httpClientFactory = httpClientFactory;
        _puppyMapper = puppyMapper;
        var rendererOptions = new RendererOptions();
        options?.Invoke(rendererOptions); ;

        RendererOptions = rendererOptions;
    }

    public async Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url)
    {
        if (url is null)
        {
            return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = "Url can't be empty", ErrorCode = "400" } });
        }

        var urlValidator = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        if (!urlValidator.IsMatch(url))
        {
            url = $"https://{url}";
        }

        if (string.IsNullOrEmpty(RendererOptions.ChromeExecutablePath))
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }
        await using var browser = await Puppeteer.LaunchAsync(options: _puppyMapper.MapToLaunchOptions(RendererOptions));
        await using var page = await browser.NewPageAsync();

        try
        {
            await page.GoToAsync(url);
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();

            var result = await page.PdfDataAsync();
            return Result.Success(result);

        }
        catch (Exception ex)
        {
            return Result.Error("An error occurred while generating the pdf");
        }
        finally
        {
            await page.CloseAsync();
        }

    }

    public async Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, Action<Core.Renderers.Configurations.PdfOptions> pdfOptions)
    {
        if (url is null)
        {
            return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = "Url can't be empty", ErrorCode = "400" } });
        }

        var urlValidator = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        if (!urlValidator.IsMatch(url))
        {
            url = $"https://{url}";
        }

        var customPdfOptions = new Core.Renderers.Configurations.PdfOptions();
        pdfOptions?.Invoke(customPdfOptions);

        if (string.IsNullOrEmpty(RendererOptions.ChromeExecutablePath))
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }

        await using var browser = await Puppeteer.LaunchAsync(options: _puppyMapper.MapToLaunchOptions(RendererOptions));
        await using var page = await browser.NewPageAsync();

        try
        {

            await page.GoToAsync(url);
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();

            var result = await page.PdfDataAsync(NormailzeHeaderFooters(customPdfOptions).Result.MappedPdfOptions);
            return Result.Success(result);

        }
        catch (Exception ex)
        {
            return Result.Error("An error occurred while generating the pdf");
        }
        finally
        {
            await page.CloseAsync();
        }


    }

    public async Task<Result<byte[]>> GeneratePdfFromUrlAsync(string url, Core.Renderers.Configurations.PdfOptions pdfOptions)
    {
        if (url is null)
        {
            return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = "Url can't be empty", ErrorCode = "400" } });
        }

        var urlValidator = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        if (!urlValidator.IsMatch(url))
        {
            url = $"https://{url}";
        }

        var optionss = pdfOptions.MappedPdfOptions;

        if (string.IsNullOrEmpty(RendererOptions.ChromeExecutablePath))
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }


        await using var browser = await Puppeteer.LaunchAsync(options: _puppyMapper.MapToLaunchOptions(RendererOptions));
        await using var page = await browser.NewPageAsync();

        try
        {
            await page.GoToAsync(url);
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();
            var result = await page.PdfDataAsync(NormailzeHeaderFooters(pdfOptions).Result.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception ex)
        {

            return Result.Error("An error occurred while generating the pdf");
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
            return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = "Html string can't be empty", ErrorCode = "400" } });
        }

        if (string.IsNullOrEmpty(RendererOptions.ChromeExecutablePath))
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }

        await using var browser = await Puppeteer.LaunchAsync(options: _puppyMapper.MapToLaunchOptions(RendererOptions));

        using var page = await browser.NewPageAsync();
        await page.EmulateMediaTypeAsync(MediaType.Screen);
        try
        {
            html = await _htmlUtils.NormalizeHtmlString(html);

            await page.SetContentAsync(html);

            await page.ImportCssStyles(html, await _htmlUtils.FindCssTagSources(html), _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient));
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();
            var result = await page.PdfDataAsync(new Core.Renderers.Configurations.PdfOptions().MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception ex)
        {

            return Result.Error("An error occurred while generating the pdf");
        }
        finally
        {
            await page.CloseAsync();
        }

    }

    public async Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, Action<Core.Renderers.Configurations.PdfOptions> options)
    {
        if (html is null)
        {
            return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = "Html string can't be empty", ErrorCode = "400" } });
        }

        if (string.IsNullOrEmpty(RendererOptions.ChromeExecutablePath))
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }

        var pdfOptions = new Core.Renderers.Configurations.PdfOptions();
        options?.Invoke(pdfOptions);

        await using var browser = await Puppeteer.LaunchAsync(options: _puppyMapper.MapToLaunchOptions(RendererOptions));

        using var page = await browser.NewPageAsync();
        await page.EmulateMediaTypeAsync(MediaType.Screen);

        try
        {
            html = await _htmlUtils.NormalizeHtmlString(html);

            await page.SetContentAsync(html);

            await page.ImportCssStyles(html, await _htmlUtils.FindCssTagSources(html), _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient));
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();
            var result = await page.PdfDataAsync(NormailzeHeaderFooters(pdfOptions).Result.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception ex)
        {

            return Result.Error("An error occurred while generating the pdf");
        }
        finally
        {
            await page.CloseAsync();
        }


    }

    public async Task<Result<byte[]>> GeneratePdfFromHtmlAsync(string html, Core.Renderers.Configurations.PdfOptions pdfOptions)
    {
        if (html is null)
        {
            return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = "Html string can't be empty", ErrorCode = "400" } });
        }

        if (string.IsNullOrEmpty(RendererOptions.ChromeExecutablePath))
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
        }


        await using var browser = await Puppeteer.LaunchAsync(options: _puppyMapper.MapToLaunchOptions(RendererOptions));

        using var page = await browser.NewPageAsync();
        await page.EmulateMediaTypeAsync(MediaType.Screen);

        try
        {
            html = await _htmlUtils.NormalizeHtmlString(html);

            await page.SetContentAsync(html);

            await page.ImportCssStyles(html, await _htmlUtils.FindCssTagSources(html), _httpClientFactory.CreateClient(ConfigConstants.PuppyHttpClient));
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions() { IdleTime = 1000 });
            var result = await page.PdfDataAsync(NormailzeHeaderFooters(pdfOptions).Result.MappedPdfOptions);
            return Result.Success(result);
        }
        catch (Exception ex)
        {

            return Result.Error("An error occurred while generating the pdf");
        }
        finally
        {
            await page.CloseAsync();
        }

    }

    private async Task<PuppySharpPdf.Core.Renderers.Configurations.PdfOptions> NormailzeHeaderFooters(PuppySharpPdf.Core.Renderers.Configurations.PdfOptions pdfOptions)
    {
        pdfOptions.HeaderTemplate = pdfOptions.HeaderTemplate != null ? await _htmlUtils.NormalizeHtmlString(pdfOptions.HeaderTemplate) : null;
        pdfOptions.FooterTemplate = pdfOptions.FooterTemplate != null ? await _htmlUtils.NormalizeHtmlString(pdfOptions.FooterTemplate) : null;

        return pdfOptions;
    }
}
