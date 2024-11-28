namespace ServiceExplorer.CommandLine.Configuration;

public interface IConfigWriter
{
    void AddFolder(string folder);
    void RemoveFolder(string folder);
}