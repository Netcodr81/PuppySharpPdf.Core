using System.ComponentModel.DataAnnotations;

namespace PuppySharpPdf.Core.Demo.Models;

public class CorgiTemplateViewModel
{

	public bool UseLocalExe { get; set; }

	[Display(Name = "Include Header")]
	public bool IncludeHeader { get; set; }

	[Display(Name = "Include Footer")]
	public bool IncludeFooter { get; set; }

	public string PdfType { get; set; } = "CardiganWelshCorgi";
	public PdfOptionsViewModel PdfOptions { get; set; }

}
