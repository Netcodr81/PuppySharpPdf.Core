using PuppySharpPdf.Core.Renderers.Configurations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PuppySharpPdf.Core.Demo.Models;

public class PdfOptionsViewModel
{
	public bool PrintBackground { get; set; } = true;
	public PaperFormatEnum FormatEnum { get; set; } = PaperFormatEnum.A4;

	[Required]
	public decimal Scale { get; set; } = 1;
	public bool DisplayHeaderFooter { get; set; }

	[DisplayName("Header Template")]
	public string HeaderTemplate { get; set; } = string.Empty;

	[DisplayName("Footer Template")]
	public string FooterTemplate { get; set; } = string.Empty;
	public bool Landscape { get; set; }

	[DisplayName("Page Ranges")]
	public string PageRanges { get; set; } = string.Empty;
	public object Width { get; set; }
	public object Height { get; set; }
	public MarginOptions MarginOptions { get; set; } = new MarginOptions();
	public bool PreferCSSPageSize { get; set; }
	public bool OmitBackground { get; set; }

	public PaperFormat GetPaperFormatType()
	{
		switch (FormatEnum)
		{
			case PaperFormatEnum.Letter:
				return PaperFormat.Letter;
			case PaperFormatEnum.Legal:
				return PaperFormat.Legal;
			case PaperFormatEnum.Tabloid:
				return PaperFormat.Tabloid;
			case PaperFormatEnum.Ledger:
				return PaperFormat.Ledger;
			case PaperFormatEnum.A0:
				return PaperFormat.A0;
			case PaperFormatEnum.A1:
				return PaperFormat.A1;
			case PaperFormatEnum.A2:
				return PaperFormat.A2;
			case PaperFormatEnum.A3:
				return PaperFormat.A3;
			case PaperFormatEnum.A4:
				return PaperFormat.A4;
			case PaperFormatEnum.A5:
				return PaperFormat.A5;
			case PaperFormatEnum.A6:
				return PaperFormat.A6;
			default:
				return PaperFormat.A4;
		}
	}
}

public enum PaperFormatEnum
{
	Letter,
	Legal,
	Tabloid,
	Ledger,
	A0,
	A1,
	A2,
	A3,
	A4,
	A5,
	A6

}