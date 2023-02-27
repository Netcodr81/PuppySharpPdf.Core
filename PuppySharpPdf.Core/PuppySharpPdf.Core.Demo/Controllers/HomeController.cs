using Microsoft.AspNetCore.Mvc;
using PuppySharpPdf.Core.Demo.Helpers;
using PuppySharpPdf.Core.Demo.Models;
using PuppySharpPdf.Core.Renderers;
using PuppySharpPdf.Core.Renderers.Configurations;
using System.Diagnostics;
using Westwind.AspNetCore.Views;

namespace PuppySharpPdf.Core.Demo.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult GeneratePdfByUrl()
    {
        ViewBag.Message = "Your application description page.";

        return View(new PdfGenerationByUrlRequest());
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePdfByUrl(PdfGenerationByUrlRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        if (request.UseLocalExe)
        {
            var pdfRendererLocal = new PuppyPdfRenderer(options =>
            {
                options.ChromeExecutablePath = Server.MapPath("~/Content/ChromeBrowser/chrome-win/chrome.exe");

            });


            if (request.DisplayHeaderFooter)
            {
                var pdfOptions = new PdfOptions()
                {
                    DisplayHeaderFooter = true,

                };

                var resultLocal = await pdfRendererLocal.GeneratePdfFromUrlAsync(request.Url, pdfOptions);
                return File(resultLocal.Value, "application/pdf", "PdfFromUrl.pdf");
            }
            else
            {
                var resultLocal = await pdfRendererLocal.GeneratePdfFromUrlAsync(request.Url);
                return File(resultLocal.Value, "application/pdf", "PdfFromUrl.pdf");
            }



        }

        var pdfRenderer = new PuppyPdfRenderer();
        var result = await pdfRenderer.GeneratePdfFromUrlAsync(request.Url);

        return File(result.Value, "application/pdf", "PdfFromUrl.pdf");
    }

    public IActionResult GeneratePdfByUrlWithCustomOptions()
    {
        return View(new PdfGenerationByUrlRequest() { PdfOptions = new PdfOptionsViewModel() });
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePdfByUrlWithCustomOptions(PdfGenerationByUrlRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        if (request.UseLocalExe)
        {
            var pdfRendererLocal = new PuppyPdfRenderer(options =>
            {
                options.ChromeExecutablePath = Server.MapPath("~/Content/ChromeBrowser/chrome-win/chrome.exe");

            });

            var resultLocal = await pdfRendererLocal.GeneratePdfFromUrlAsync(request.Url, options =>
            {
                options.PrintBackground = request.PdfOptions.PrintBackground;
                options.Format = request.PdfOptions.GetPaperFormatType();
                options.Scale = request.PdfOptions.Scale;
                options.DisplayHeaderFooter = request.PdfOptions.DisplayHeaderFooter;
                options.HeaderTemplate = request.PdfOptions.HeaderTemplate;
                options.FooterTemplate = request.PdfOptions.FooterTemplate;
                options.Landscape = request.PdfOptions.Landscape;
                options.PageRanges = request.PdfOptions.PageRanges;
                options.Width = request.PdfOptions.Width;
                options.Height = request.PdfOptions.Height;
                options.MarginOptions = request.PdfOptions.MarginOptions;
                options.PreferCSSPageSize = request.PdfOptions.PreferCSSPageSize;
                options.OmitBackground = request.PdfOptions.OmitBackground;
            });

            return File(resultLocal.Value, "application/pdf", "PdfFromUrlWithCustomOptions.pdf");
        }

        var pdfRenderer = new PuppyPdfRenderer();
        var result = await pdfRenderer.GeneratePdfFromUrlAsync(request.Url, options =>
        {
            options.PrintBackground = request.PdfOptions.PrintBackground;
            options.Format = request.PdfOptions.GetPaperFormatType();
            options.Scale = request.PdfOptions.Scale;
            options.DisplayHeaderFooter = request.PdfOptions.DisplayHeaderFooter;
            options.HeaderTemplate = request.PdfOptions.HeaderTemplate;
            options.FooterTemplate = request.PdfOptions.FooterTemplate;
            options.Landscape = request.PdfOptions.Landscape;
            options.PageRanges = request.PdfOptions.PageRanges;
            options.Width = request.PdfOptions.Width;
            options.Height = request.PdfOptions.Height;
            options.MarginOptions = request.PdfOptions.MarginOptions;
            options.PreferCSSPageSize = request.PdfOptions.PreferCSSPageSize;
            options.OmitBackground = request.PdfOptions.OmitBackground;
        });

        return File(result.Value, "application/pdf", "PdfFromUrlWithCustomOptions.pdf");
    }


    public IActionResult GeneratePdfUsingRazorTemplate()
    {
        return View(new CorgiTemplateViewModel() { PdfOptions = new PdfOptionsViewModel { PreferCSSPageSize = true } });
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePdfUsingRazorTemplate(CorgiTemplateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var html = ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/CorgiInfo.cshtml", model, this.ControllerContext);
        var fileName = model.PdfType == "CardiganWelshCorgi" ? "CardiganWelshCorgi_razor.pdf" : "PembrokeWelshCorgi_razor.pdf";

        if (model.UseLocalExe)
        {
            var pdfRendererLocal = new PuppyPdfRenderer(options =>
            {
                options.ChromeExecutablePath = Server.MapPath("~/Content/ChromeBrowser/chrome-win/chrome.exe");

            });

            if (model.IncludeFooter || model.IncludeHeader)
            {
                var pdfOptions = new PdfOptions
                {
                    DisplayHeaderFooter = true,
                    HeaderTemplate = model.IncludeHeader ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Header.cshtml", model, this.ControllerContext) : null,
                    FooterTemplate = model.IncludeFooter ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Footer.cshtml", null, this.ControllerContext) : null,
                    MarginOptions = new MarginOptions
                    {
                        Top = "160px",
                        Bottom = "100px",
                        Left = "0px",
                        Right = "0px"

                    }
                };

                var pdfWithHeader = await pdfRendererLocal.GeneratePdfFromHtmlAsync(html.Result, pdfOptions);
                return File(pdfWithHeader.Value, "application/pdf", fileName);
            }

            var noHeaderFooterPdfOptions = new PdfOptions
            {
                DisplayHeaderFooter = false,
                MarginOptions = new MarginOptions
                {
                    Top = "40px",
                    Bottom = "80px",
                    Left = "0px",
                    Right = "0px"

                }
            };

            var resultLocal = await pdfRendererLocal.GeneratePdfFromHtmlAsync(html.Result, noHeaderFooterPdfOptions);

            return File(resultLocal.Value, "application/pdf", fileName);
        }

        var pdfRenderer = new PuppyPdfRenderer();

        if (model.IncludeFooter || model.IncludeHeader)
        {
            var pdfOptions = new PdfOptions
            {
                DisplayHeaderFooter = true,
                HeaderTemplate = model.IncludeHeader ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Header.cshtml", model, this.ControllerContext) : null,
                FooterTemplate = model.IncludeFooter ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Footer.cshtml", null, this.ControllerContext) : null,
                MarginOptions = new MarginOptions
                {
                    Top = "160px",
                    Bottom = "100px",
                    Left = "0px",
                    Right = "0px"

                }
            };

            var pdfWithHeader = await pdfRenderer.GeneratePdfFromHtmlAsync(html.Result, pdfOptions);
            return File(pdfWithHeader.Value, "application/pdf", fileName);
        }

        var noHeaderPdfOptions = new PdfOptions
        {
            DisplayHeaderFooter = false,
            MarginOptions = new MarginOptions
            {
                Top = "40px",
                Bottom = "80px",
                Left = "0px",
                Right = "0px"

            }
        };
        var result = await pdfRenderer.GeneratePdfFromHtmlAsync(html.Result, noHeaderPdfOptions);
        return File(result.Value, "application/pdf", fileName);
    }

    public IActionResult GeneratePdfUsingHtmlTemplate()
    {
        return View(new CorgiTemplateViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePdfUsingHtmlTemplate(CorgiTemplateViewModel model)
    {


        var html = model.PdfType == "CardiganWelshCorgi" ? System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Views/Shared/Templates/CardiganWelshCorgi.html") :
            System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Views/Shared/Templates/PembrokeWelshCorgi.html");
        var pdfRenderer = new PuppyPdfRenderer();
        var fileName = model.PdfType == "CardiganWelshCorgi" ? "CardiganWelshCorgi_html.pdf" : "PembrokeWelshCorgi_html.pdf";

        if (model.IncludeFooter || model.IncludeHeader)
        {
            var pdfOptions = new PdfOptions
            {
                DisplayHeaderFooter = true,
                HeaderTemplate = model.IncludeHeader ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Header.cshtml", model, this.ControllerContext) : null,
                FooterTemplate = model.IncludeFooter ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Footer.cshtml", null, this.ControllerContext) : null,
                MarginOptions = new MarginOptions
                {
                    Top = "160px",
                    Bottom = "160px",
                    Left = "0px",
                    Right = "0px"

                }
            };



            var results = await pdfRenderer.GeneratePdfFromHtmlAsync(html, pdfOptions);

            return File(results.Value, "application/pdf", fileName);
        }

        var noHeaderPdfOptions = new PdfOptions
        {
            DisplayHeaderFooter = false,
            MarginOptions = new MarginOptions
            {
                Top = "40px",
                Bottom = "80px",
                Left = "0px",
                Right = "0px"

            }
        };

        var result = await pdfRenderer.GeneratePdfFromHtmlAsync(html, noHeaderPdfOptions);

        return File(result.Value, "application/pdf", fileName);
    }

    public IActionResult GeneratePdfUsingHtmlTemplateWithCustomOptions()
    {
        return View(new CorgiTemplateViewModel() { PdfOptions = new PdfOptionsViewModel() });
    }

    [HttpPost]
    public IActionResult GeneratePdfUsingHtmlTemplateWithCustomOptions(CorgiTemplateViewModel model)
    {
        return View();
    }

    public IActionResult GeneratePdfUsingRazorTemplateWithCustomOptions()
    {
        return View(new CorgiTemplateViewModel() { PdfOptions = new PdfOptionsViewModel() });
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePdfUsingRazorTemplateWithCustomOptions(CorgiTemplateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var html = await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/CorgiInfo.cshtml", model, this.ControllerContext);
        var fileName = model.PdfType == "CardiganWelshCorgi" ? "CardiganWelshCorgi_razor.pdf" : "PembrokeWelshCorgi_razor.pdf";
        var pdfRenderer = new PuppyPdfRenderer();

        var pdfOptions = new PdfOptions
        {
            PrintBackground = model.PdfOptions.PrintBackground,
            Format = model.PdfOptions.GetPaperFormatType(),
            Scale = model.PdfOptions.Scale,
            DisplayHeaderFooter = model.PdfOptions.DisplayHeaderFooter,
            HeaderTemplate = model.IncludeHeader ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Header.cshtml", model, this.ControllerContext) : null,
            FooterTemplate = model.IncludeFooter ? await ViewRenderer.RenderViewToStringAsync("~/Views/Shared/Templates/Footer.cshtml", null, this.ControllerContext) : null,
            Landscape = model.PdfOptions.Landscape,
            PageRanges = model.PdfOptions.PageRanges,
            Width = model.PdfOptions.Width,
            Height = model.PdfOptions.Height,
            MarginOptions = model.PdfOptions.MarginOptions,
            PreferCSSPageSize = model.PdfOptions.PreferCSSPageSize,
            OmitBackground = model.PdfOptions.OmitBackground
        };

        if (model.UseLocalExe)
        {
            var pdfRendererLocal = new PuppyPdfRenderer(options =>
            {
                options.ChromeExecutablePath = Server.MapPath("~/Content/ChromeBrowser/chrome-win/chrome.exe");

            });

            var resultFromLocalExe = await pdfRenderer.GeneratePdfFromHtmlAsync(html, pdfOptions);
            return File(resultFromLocalExe.Value, "application/pdf", fileName);

        }





        var result = await pdfRenderer.GeneratePdfFromHtmlAsync(html, pdfOptions);
        return File(result.Value, "application/pdf", fileName);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
