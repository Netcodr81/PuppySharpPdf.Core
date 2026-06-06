using Microsoft.Playwright;

namespace PuppySharpPdf.Core.Renderers;

public interface IPlaywrightBrowserProvider : IAsyncDisposable
{
    Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken = default);
}
