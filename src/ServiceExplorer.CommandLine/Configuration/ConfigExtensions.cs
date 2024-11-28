using ServiceExplorer.CommandLine.IO;

namespace ServiceExplorer.CommandLine.Configuration;

public static class ConfigExtensions
{
    public static void WriteFolders(this IOutput output, IConfig config)
    {
        if (config.Folders.Length > 0)
        {
            output.WriteLine("Folders:");

            foreach (var folder in config.Folders)
                output.WriteLine($"    * {folder}");
        }
    }
}
