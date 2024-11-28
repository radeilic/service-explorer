namespace ServiceExplorer.CommandLine.IO;

public interface IOutput
{
    void WriteLine(string text);
    void Write(string text);
    int TotalLinesAvailable { get; }
    int LinePosition { get; set; }

    int TotalColumnsAvailable { get; }

    ConsoleKeyInfo ReadKey();
}
