namespace PuppySharpPdf.Core.Renderers.Configurations;
public class MarginOptions
{
	public MarginOptions()
	{
	}

	/// <summary>
	/// Top margin, accepts values labeled with units.
	/// </summary>
	public string Top { get; set; }

	/// <summary>
	/// Left margin, accepts values labeled with units.
	/// </summary>
	public string Left { get; set; }

	/// <summary>
	/// Bottom margin, accepts values labeled with units.
	/// </summary>
	public string Bottom { get; set; }

	/// <summary>
	/// Right margin, accepts values labeled with units.
	/// </summary>
	public string Right { get; set; }

	public PuppeteerSharp.Media.MarginOptions MappedMarginOptions => new PuppeteerSharp.Media.MarginOptions
	{
		Top = this.Top,
		Left = this.Left,
		Bottom = this.Bottom,
		Right = this.Right
	};
}
