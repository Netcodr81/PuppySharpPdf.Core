using Microsoft.AspNetCore.Mvc;
using PuppySharpPdf.Core.Demo.Models;
using PuppySharpPdf.Core.Interfaces;
using PuppySharpPdf.Core.Renderers.Configurations;
using System.Diagnostics;
using Westwind.AspNetCore.Views;

namespace PuppySharpPdf.Core.Demo.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    readonly IPuppyPdfRenderer _pdfRenderer;

    public HomeController(ILogger<HomeController> logger, IPuppyPdfRenderer pdfRenderer)
    {
        _pdfRenderer = pdfRenderer;
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

            if (request.DisplayHeaderFooter)
            {
                var pdfOptions = new PdfOptions()
                {
                    DisplayHeaderFooter = true,

                };

                var resultLocal = await _pdfRenderer.GeneratePdfFromUrlAsync(request.Url, pdfOptions);
                return File(resultLocal.Value, "application/pdf", "PdfFromUrl.pdf");
            }
            else
            {
                var resultLocal = await _pdfRenderer.GeneratePdfFromUrlAsync(request.Url);
                return File(resultLocal.Value, "application/pdf", "PdfFromUrl.pdf");
            }



        }


        var result = await _pdfRenderer.GeneratePdfFromUrlAsync(request.Url);

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


            var resultLocal = await _pdfRenderer.GeneratePdfFromUrlAsync(request.Url, options =>
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


        var result = await _pdfRenderer.GeneratePdfFromUrlAsync(request.Url, options =>
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

                var pdfWithHeader = await _pdfRenderer.GeneratePdfFromHtmlAsync(html.Result, pdfOptions);
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

            var resultLocal = await _pdfRenderer.GeneratePdfFromHtmlAsync(html.Result, noHeaderFooterPdfOptions);

            return File(resultLocal.Value, "application/pdf", fileName);
        }



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

            var pdfWithHeader = await _pdfRenderer.GeneratePdfFromHtmlAsync(html.Result, pdfOptions);
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
        var result = await _pdfRenderer.GeneratePdfFromHtmlAsync(html.Result, noHeaderPdfOptions);
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



            var results = await _pdfRenderer.GeneratePdfFromHtmlAsync(html, pdfOptions);

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

        var result = await _pdfRenderer.GeneratePdfFromHtmlAsync(html, noHeaderPdfOptions);

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


            var resultFromLocalExe = await _pdfRenderer.GeneratePdfFromHtmlAsync(html, pdfOptions);
            return File(resultFromLocalExe.Value, "application/pdf", fileName);

        }





        var result = await _pdfRenderer.GeneratePdfFromHtmlAsync(html, pdfOptions);
        return File(result.Value, "application/pdf", fileName);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
