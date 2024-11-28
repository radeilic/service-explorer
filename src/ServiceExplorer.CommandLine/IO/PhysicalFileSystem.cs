namespace ServiceExplorer.CommandLine.IO;

public class PhysicalFileSystem : IFileSystem
{
    public bool ExistsDirectory(string path)
        => Directory.Exists(path);
}
