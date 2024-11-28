namespace ServiceExplorer.Infrastructure.Drawing;

public class ChartResult
{
    public ChartResult(string outputFilename)
    {
        OutputFilename = outputFilename;
    }

    public string OutputFilename { get; }
}
