using System.Diagnostics;

namespace ServiceExplorer.Infrastructure.Drawing;

public class ShowChartResult : ChartResult
{
    private static Process? _currentProcess;

    public ShowChartResult(string outputFilename, Process process)
        : base (outputFilename)
    {
        _currentProcess = process;
    }
    
    public static void Kill() => _currentProcess?.Kill();
}
