# PuppySharpPdf.Core
This is a wrapper for the Puppeteer Sharp package. This package abstracts the complexity of using Puppeteer Sharp making it easier to generate PDFs in a .NET Core applications. With this package you will be able to generate PDF's from a URL, using a Razor (.cshtml) file, or an Html File.

## License

MIT

## Getting Started

You can install the package via NugGet package manager just search for *PuppySharpPdf.Core*. You can also intall via powershell using the following command.
<br>
<br>

```powershell
Install-Package PuppySharpPdf.Core -Version 1.0.0
```
<br>
Or via the dotnet CLI.
<br>
<br>

```bash
dotnet add package PuppySharpPdf.Core --version 1.0.0
```

### 1. Register Services
You will need to register the PuppySharpPdfCore service in your application. In this service you will register a *HttpClient* and the RendererOptions class (Optional. If not configured the defaults will be applied).

<br/>
The following options are available in the RendererOptions class:
<br/>
<br/>

| Option | Description |
| ------ | ----------- |
| IgnoreHTTPSErrors | Whether to ignore HTTPS errors during navigation. Defaults to false. |
| Headless | Whether to run Chromium in headless mode. Defaults to true. |
| ChromeExecutablePath | Path to a Chromium or Chrome executable to run instead of bundled Chromium. If executablePath is a relative path, then it is resolved relative to current working directory. |
| Args | Addtional arguments to pass to the browser instance. The list of Chromium flags can be found here https://www.chromium.org/developers/how-tos/run-chromium-with-flags/ |
| Timeout | Maximum time in milliseconds to wait for the browser instance to start. Defaults to 30000 (30 seconds). Pass 0 to disable timeout. |

### HttpClient configuration only

```csharp
builder.Services.AddPuppySharpPdfCore(config =>
{
    config.BaseAddress = new Uri("http://localhost:5136");
});
```
<br>

The *BaseAddress* Property is the only property of the *HttpClient* that is required. You can configure the other options but they are not required.

<br>

### HttpClient and RendererOptions configuration

```csharp
// Configuration used to point to a local Chromium instance
builder.Services.AddPuppySharpPdfCore(renderer =>
{
    renderer.ChromeExecutablePath = AppContext.BaseDirectory + "Chrome\\chrome-win\\chrome.exe";
},
    httpClient =>
{
    httpClient.BaseAddress = new Uri("http://localhost:5136");
});
```

<br>

The PuppySharpPdfCore service will register the PuppyPdfRenderer class as a scoped service. It can be injected throught the use of the IPuppyPdfRenderer interface.

<br>

```csharp
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    readonly IPuppyPdfRenderer _pdfRenderer;

    public HomeController(ILogger<HomeController> logger, IPuppyPdfRenderer pdfRenderer)
    {
        _pdfRenderer = pdfRenderer;
        _logger = logger;
    }

    ...

}
```
## Usage

PuppySharpPdf is meant to be easy to use. The following will show you how to use the package
followed by some gotchas about the Chromium based engine to look out for.


### Generating a PDF from a URL
To generate a PDF from a URL you will need to call the *GeneratePdfFromUrl* method on the *GeneratePdfFromUrlAsync* class. The *GeneratePdfFromUrlAsync* method takes two parameters, the first is the URL to generate the PDF from and the second is any PDF configurations using the *PdfOptions* class. The renderer follows the Result Pattern using the [Ardalis.Result](https://github.com/ardalis/result) nuget package and will return a ***Result<byte[]>***. This pattern allows you to check if
the PDF was successfully Rendered. To get the byte array value, call the *.Value* property on the returned object. From here you can save the PDF to a file or stream it to the browser. Below is an example of rendering a PDF from a URL in an Asp.NET Core 7 MVC application.

```csharp
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    readonly IPuppyPdfRenderer _pdfRenderer;

    public HomeController(ILogger<HomeController> logger, IPuppyPdfRenderer pdfRenderer)
    {
        _pdfRenderer = pdfRenderer;
        _logger = logger;
    }

    public async Task<IActionResult> GeneratePdfFromUrlWithNoOptions(){
        
        var pdf = await _pdfRenderer.GeneratePdfFromUrlAsync("www.google.com");

        return File(pdf.Value, "application/pdf", "PdfFromUrl.pdf");
    }

    public async Task<IActionResult> GeneratePdfFromUrlWithCustomPdfOptions(){
        // The following uses the PdfOptions class to add custom pdf configurations. See below for more details.
        var pdf = await _pdfRenderer.GeneratePdfFromUrlAsync("www.google.com", options => {
            options.PrintBackground = true;
            options.Landscape = true;
            ...
        });
    }

}
```

<br/>

The following options are available in the PdfOptions class:
<br/>
<br/>

| Option | Description |
| ------ | ----------- |
| PrintBackground  | Print background graphics. Defaults to true. |
| Format  | Uses the PaperFormat class to set the width and height of the pdf. If set, takes priority over the Width and Height options. |
| Scale  | Scale of the webpage rendering. Defaults to <c>1</c>. Scale amount must be between 0.1 and 2. |
| DisplayHeaderFooter  | Display header and footer. Defaults to false |
| HeaderTemplate  | HTML template for the print header. |
| FooterTemplate  | HTML template for the print footer. |
| Landscape | Paper orientation. Defaults to false. |
| PageRanges | Paper ranges to print, e.g., '1-5, 8, 11-13'. Defaults to the empty string, which means print all pages. |
| Width | Paper width, accepts values labeled with units. |
| Height | Paper height, accepts values labeled with units. |
| MarginOptions | Uses the MarginOptions class to set the top, left, right, and bottom margins. |
| PreferCSSPageSize | Give any CSS @page size declared in the page priority over what is declared in width and height or format options. Defaults to false, which will scale the content to fit the paper size. |
| OmitBackground | hides default white background and allows generating PDFs with transparency. Defaults to false. |

<br/>
The following is all the available options in the PaperFormat class:
<br/>
<br/>

|Options | Description |
| ------ | ----------- |
| Letter | 8.5in x 11in |
| Legal | 8.5in x 14in |
| Tabloid | 11in x 17in |
| Ledger | 17in x 11in |
| A0 | 33.1in x 46.8in |
| A1 | 23.4in x 33.1in |
| A2 | 16.5in x 23.4in |
| A3 | 11.7in x 16.5in |
| A4 | 8.27in x 11.7in |
| A5 | 5.83in x 8.27in |
| A6 | 4.13in x 5.83in |

<br/>
The following is all the available options in the MarginOptions class:
<br/>
<br/>

Options | Description |
| ------ | ----------- |
| Top | Top margin, accepts values labeled with units. |
| Bottom | Bottom margin, accepts values labeled with units. |
| Left | Left margin, accepts values labeled with units. |
| Right | Right margin, accepts values labeled with units. |

### Generating a PDF from a Razor (.cshtml) / Html file

To generate a PDF from a Razor/Html file you will need to call the *GeneratePdfFromHtmlAsync* method on the *PuppyPdfRenderer* class. The *GeneratePdfFromHtmlAsync* method takes two parameters, the first is the HTML string (created by the Razor/Html file or a just a valid HTML string) and the second is any PDF configurations using the *PdfOptions* class. The renderer will return a byte array of the PDF.
From here you can save the PDF to a file or stream it to the browser. Below is an example of rendering a PDF from a Razor/HTML in an Asp.NET Core 7 MVC application. In this case I am using the WestWind.AspNetCore nuget package to render an html string from
a Razor file and the build in File class to read in an HTML file to a string.

#### HTML File

```csharp
using System.IO.File;

...

var html = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Views/Reports/Report.html");

var pdf = await _pdfRenderer.GeneratePdfFromHtmlAsync(html);

return File(pdf.Value, "application/pdf", "PdfFromUrl.pdf");
```

#### Razor File

```csharp
using Westwind.AspNetCore;

var pdfRenderer = new PuppyPdfRenderer();

var html =  await ViewRenderer.RenderPartialViewAsync("../Shared/Templates/PdfTempalte.cshtml");

var pdf = await _pdfRenderer.GeneratePdfFromHtmlAsync(html);

return File(pdf.Value, "application/pdf", "PdfFromUrl.pdf");
```
<br>

The *GeneratePdfFromHtmlAsync* accepts the same *PdfOptions* class that the *GeneratePdfFromUrlAsync* method accepts. Pass the *PdfOptions* class in as the second parameter if desired just as you did in the example above with the *GeneratePdfFromUrlAsync*.


### Gotchas

There are a few gotchas to be aware of when using this package. All these gotchas are due to restrictions of the underlying Chromium browser. 

#### Headers and Footers

The header and footer templates do not inherit any body styles. This means in order to style your header or footer, the styles will need to be added directly to the header or footer template using inline styles or classes within a style tag. Below is an example of a header template with inline styles. You will also need to add top and bottom page margins respectively
using the MarginsOption class in order for the header and footer to be visible. You may need to play around with the amount a margin to add based on the size of your header and footer.

#### Header Template Example

```html
<style>
    @*This header style ensures the header aligns to the top of the page and the background color will be visible*@
    
    #header {
        -webkit-print-color-adjust: exact;
        padding: 0 !important;
        height: 100% !important;
    }

    .pdf-header {
        display: flex;
        flex-direction: row;
        width: 100%;
        padding: 10px;
        margin-bottom: 80px;
        align-content: center;
        background-color: #0a54a3;
    }

    .header-title {
        display: flex;
        flex-direction: row;
        width: 100%;
        align-items: center;
        margin-left: 20px;
    }
</style>

<div class="pdf-header">
    <div>
        <img src="~/Content/images/corgi.png" width="80" height="80" />
    </div>
    <div class="header-title">
        <p class="mt-3" style="font-weight:bold;font-size:18px;color:white">Header Title</p>
    </div>
</div>
```


#### Footer Template Example

```html
<style>
    @*This footer style ensures the footer aligns to the bottom of the page and the background color will be visible*@
    #footer {
        -webkit-print-color-adjust: exact;
        padding: 0 !important;
        height: 100% !important;
    }
</style>

<!--A simple footer -->
@*<div style="font-size: 10px; text-align: center; width: 100%; padding: 20px;">
        Page <span class="pageNumber"></span> of <span class="totalPages"></span>
    </div>*@

<!--A simple footer with a background-->
<div style="font-size: 10px; text-align: center; width: 100%; padding: 20px; background-color: #b3b1b1; ">
    Page <span class="pageNumber"></span> of <span class="totalPages"></span>
</div>

<!-- A more complex footer with an image and a link -->
@*<div style="font-size: 10px; text-align: center; width: 100%; margin-top: 20px;">
    <img src="logo.png" alt="Company Logo" style="height: 20px; width: auto; margin-right: 10px;">
    � 2023 My Company, Inc. | <a href="https://www.mycompany.com">www.mycompany.com</a> | Page <span class="pageNumber"></span> of <span class="totalPages"></span>
</div>*@

<!-- A footer with a background color and border -->
@*<div style="font-size: 10px; text-align: center; width: 100%; margin-top: 20px; background-color: #f2f2f2; border-top: 1px solid #ccc; padding-top: 10px;">
    Page <span class="pageNumber"></span> of <span class="totalPages"></span>
</div>*@

```
<br/>

The id targeting the header element *#header*  and footer element #footer is specific to the Chromium rendering engine. If you want your header to be flush with the top of the page, the footer to be flush with the bottom of the page
and the background color to be visible, you will need to add this style to your header and footer templates.

The following classes can be uses in the header and footer templates:

| Class | Description |
| ------ | ----------- |
| pageNumber | The current page number |
| totalPages | The total number of pages |
| date | The current date |
| title | The title of the page |
| url | The url of the page |

#### Keeping Content Together and Page Breaks

Keeping content together and adding page breaks to the main body of the document is possible using the *page-break-before* and *page-break-after* CSS properties. Below is an example of a table with a page break after the table.

```html
Keeping Content Together

<table style="page-break-inside: avoid;">
	<tr>
		<td>Row 1</td>
		<td>Row 1</td>
	</tr>
	<tr>
		<td>Row 2</td>
		<td>Row 2</td>
	</tr>
	<tr>
		<td>Row 3</td>
		<td>Row 3</td>
	</tr>
```
<br/>

```html
Adding Page Breaks

<table style="page-break-after: always;">
	<tr>
		<td>Row 1</td>
		<td>Row 1</td>
	</tr>
	<tr>
		<td>Row 2</td>
		<td>Row 2</td>
	</tr>
	<tr>
		<td>Row 3</td>
		<td>Row 3</td>
	</tr>

```
<br>

More information about the page break class can be found here [https://www.w3schools.com/cssref/pr_print_pageba.php](https://www.w3schools.com/cssref/pr_print_pageba.php)

#### Images

Due to the security sandboxing employed by the Chromium browser, images will not be rendered unless they are encoded into a base64 string. This package has abstracted the complexity of doing this for you. There are just a few things you will need to do in order for the image to be rendered.
If you are rendering an image stored in a local folder, you will need to ensure the image source starts at the root of the project like below:


```html
  *** Be sure to include the image extension ***

   <div class="w-100 mb-3 w-100">
        <img src="~/Content/images/image.jpg" />
    </div>
```

<br/>

Any images pulled from a CDN or external URL should be picked up and rendered. If you have any issues, ensure the image URL is a valid URL and that the image is publicly accessible.

#### Chromium Engine

For this package to work, the Chromium engine must be accessible. This package will attempt to find the Chromium engine and download it if not specified in the *RendererOptions.ChromeExecutablePath* property.
One issue you might run into is your app runs fine locally without specifying a Chromium engine path, but when deployed to a server, your application fails. This is most likely due to your server not allowing
the package to download the Chromium engine. To resolve this issue, you will need to download the Chromium engine and specify the path to the executable in the *RendererOptions.ChromeExecutablePath* property. This package requires
a minimum Chromium version of 100 to work. To download the latest Chromium engine, visit [https://chromium.woolyss.com/download/en/](https://chromium.woolyss.com/download/en/)
<br/>
<br/>






