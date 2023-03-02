namespace PuppySharpPdf.Core.Demo.Helpers;

public static class Server
{
    public static string MapPath(string path)
    {
        return Path.Combine((string)AppDomain.CurrentDomain.GetData("ContentRootPath"), path);
    }
}
