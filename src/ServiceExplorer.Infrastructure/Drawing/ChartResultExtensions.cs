using System.Diagnostics;

namespace ServiceExplorer.Infrastructure.Drawing;

public static class ChartResultExtensions
{
    public static ShowChartResult Show(this ChartResult result)
    {
        var process = Process.Start("cmd", $"/c start {result.OutputFilename}");
        return new ShowChartResult(result.OutputFilename, process);
    }
}