using System.Diagnostics.CodeAnalysis;

namespace ServiceExplorer.CommandLine.IO;

public class PhysicalConsoleOutput : IOutput
{
    [ExcludeFromCodeCoverage]
    public int TotalLinesAvailable => Console.WindowHeight;

    [ExcludeFromCodeCoverage]
    public int LinePosition
    {
        get => Console.CursorTop;
        set => Console.CursorTop = value;
    }

    [ExcludeFromCodeCoverage]
    public int TotalColumnsAvailable => Console.WindowWidth;

    [ExcludeFromCodeCoverage]
    public ConsoleKeyInfo ReadKey() => Console.ReadKey();

    public void Write(string text) => Console.Write(text);

    public void WriteLine(string text) => Console.WriteLine(text);
}
