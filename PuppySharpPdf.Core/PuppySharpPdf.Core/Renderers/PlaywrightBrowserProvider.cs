using Microsoft.Playwright;
using PuppySharpPdf.Core.Renderers.Configurations;

namespace PuppySharpPdf.Core.Renderers;

internal sealed class PlaywrightBrowserProvider : IPlaywrightBrowserProvider
{
    private readonly BrowserTypeLaunchOptions _launchOptions;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public PlaywrightBrowserProvider(RendererOptions rendererOptions)
    {
        _launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = rendererOptions.Headless,
            ExecutablePath = rendererOptions.ChromeExecutablePath,
            Args = rendererOptions.Args,
            Timeout = rendererOptions.Timeout
        };
    }

    public async Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken = default)
    {
        if (_browser is not null)
        {
            return _browser;
        }

        await _initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (_browser is not null)
            {
                return _browser;
            }

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(_launchOptions);
            return _browser;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;
        _initializationLock.Dispose();
    }
}
