using PuppySharpPdf.Core.Renderers.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuration used to point to a local Chromium instance
//builder.Services.AddPuppySharpPdfCore(renderer =>
//{
//    renderer.ChromeExecutablePath = AppContext.BaseDirectory + "Chrome\\chrome-win\\chrome.exe";
//},
//    config =>
//{
//    config.BaseAddress = new Uri("http://localhost:5136");
//});

// Configuration used to download and cache Chromium instance
builder.Services.AddPuppySharpPdfCore(config =>
    {
        config.BaseAddress = new Uri("http://localhost:5136");
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
