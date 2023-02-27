namespace PuppySharpPdf.Core.Renderers.Configurations;
public class RendererOptions
{
	/// <summary>
	/// Ignore HTTPS errors during navigation. Defaults to false.
	/// </summary>
	public bool IgnoreHTTPSErrors { get; set; }

	/// <summary>
	/// Run browser in headless mode. Defaults to true.
	/// </summary>
	public bool Headless { get; set; } = true;

	/// <summary>
	/// Path to a Chromium or Chrome executable to run instead of bundled Chromium. If executablePath is a relative path, then it is resolved relative to current working directory.
	/// </summary>
	public string ChromeExecutablePath { get; set; }

	/// <summary>
	/// Addtional arguments to pass to the browser instance. The list of Chromium flags can be found here https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
	/// </summary>
	public string[] Args { get; set; } = Array.Empty<string>();

	/// <summary>
	/// Maximum time in milliseconds to wait for the browser instance to start. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
	/// </summary>
	public int Timeout { get; set; } = 30000;
}
