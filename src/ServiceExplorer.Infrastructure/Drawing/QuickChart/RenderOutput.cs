namespace ServiceExplorer.Infrastructure.Drawing.QuickChart;

class RenderOutput
{
    public RenderOutput(string url, string data)
    {
        Url = url;
        Data = data;
    }

    public string Url { get; }
    public string Data { get; }
}