using Microsoft.Playwright;

namespace PuppySharpPdf.Core.Renderers.Configurations;

public class MarginOptions
{
    /// <summary>
    /// Top margin, accepts values labeled with units.
    /// </summary>
    public string? Top { get; set; }

    /// <summary>
    /// Left margin, accepts values labeled with units.
    /// </summary>
    public string? Left { get; set; }

    /// <summary>
    /// Bottom margin, accepts values labeled with units.
    /// </summary>
    public string? Bottom { get; set; }

    /// <summary>
    /// Right margin, accepts values labeled with units.
    /// </summary>
    public string? Right { get; set; }

    public Margin MappedMarginOptions => new()
    {
        Top = Top,
        Left = Left,
        Bottom = Bottom,
        Right = Right
    };
}
