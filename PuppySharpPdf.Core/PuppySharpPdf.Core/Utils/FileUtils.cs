using System.Reflection;

namespace PuppySharpPdf.Core.Utils;
internal class FileUtils
{
	internal string RenderCssContentFromFileToString(string pathToCssFile)
	{
		string assemblyPath = Assembly.GetExecutingAssembly().Location;
		string directoryPath = Path.GetDirectoryName(assemblyPath);


		var css = System.IO.File.ReadAllText(Path.Combine(directoryPath, pathToCssFile.Replace("~", string.Empty)));
		return css;
	}
}
