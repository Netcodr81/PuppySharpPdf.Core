using Ardalis.Result;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using PuppySharpPdf.Core.Common;
using PuppySharpPdf.Core.Common.Mapping;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Renderers.Configurations;
using System.Text.RegularExpressions;


namespace PuppySharpPdf.Core.Renderers; public class PuppyPdfRenderer : IPuppyPdfRenderer
{
    readonly IHtmlUtils _htmlUtils;
    public RendererOptions RendererOptions { get; }

    public PuppyPdfRenderer(IHtmlUtils htmlUtils)
    {

        RendererOptions = new RendererOptions();
        _htmlUtils = htmlUtils;
    }

    public PuppyPdfRenderer(Action<RendererOptions> options, IHtmlUtils htmlUtils)
    {
        _htmlUtils = htmlUtils;
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
        await using var browser = await Puppeteer.LaunchAsync(options: PuppyMapper.MapToLaunchOptions(RendererOptions));
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

        await using var browser = await Puppeteer.LaunchAsync(options: PuppyMapper.MapToLaunchOptions(RendererOptions));
        await using var page = await browser.NewPageAsync();

        try
        {

            await page.GoToAsync(url);
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();

            var result = await page.PdfDataAsync(customPdfOptions.MappedPdfOptions);
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


        await using var browser = await Puppeteer.LaunchAsync(options: PuppyMapper.MapToLaunchOptions(RendererOptions));
        await using var page = await browser.NewPageAsync();

        try
        {
            await page.GoToAsync(url);
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();
            var result = await page.PdfDataAsync(pdfOptions.MappedPdfOptions);
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

        await using var browser = await Puppeteer.LaunchAsync(options: PuppyMapper.MapToLaunchOptions(RendererOptions));

        using var page = await browser.NewPageAsync();
        await page.EmulateMediaTypeAsync(MediaType.Screen);
        try
        {
            html = _htmlUtils.NormalizeHtmlString(html);

            await page.SetContentAsync(html);

            await page.ImportCssStyles(html, _htmlUtils.FindCssTagSources(html));
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

        await using var browser = await Puppeteer.LaunchAsync(options: PuppyMapper.MapToLaunchOptions(RendererOptions));

        using var page = await browser.NewPageAsync();
        await page.EmulateMediaTypeAsync(MediaType.Screen);

        try
        {
            html = _htmlUtils.NormalizeHtmlString(html);

            await page.SetContentAsync(html);

            await page.ImportCssStyles(html, _htmlUtils.FindCssTagSources(html));
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync();
            var result = await page.PdfDataAsync(pdfOptions.MappedPdfOptions);
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


        await using var browser = await Puppeteer.LaunchAsync(options: PuppyMapper.MapToLaunchOptions(RendererOptions));

        using var page = await browser.NewPageAsync();
        await page.EmulateMediaTypeAsync(MediaType.Screen);

        try
        {
            html = _htmlUtils.NormalizeHtmlString(html);

            await page.SetContentAsync(html);

            await page.ImportCssStyles(html, _htmlUtils.FindCssTagSources(html));
            await page.SetJavaScriptEnabledAsync(true);
            await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions() { IdleTime = 1000 });
            var result = await page.PdfDataAsync(pdfOptions.MappedPdfOptions);
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
}
