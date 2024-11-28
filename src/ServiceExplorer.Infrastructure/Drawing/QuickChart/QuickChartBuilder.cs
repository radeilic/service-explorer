using ServiceExplorer.Domain;
using Svg;
using System.Text;

namespace ServiceExplorer.Infrastructure.Drawing.QuickChart;

public partial class QuickChartBuilder : IChartBuilder
{
    public ChartResult Build<TParentNode, TChildNode>(ChartBuilderInput<TParentNode, TChildNode> input)
      where TParentNode : INodeInfo<TChildNode>
      where TChildNode : INodeInfo
    {
        var digraph = new Digraph();

        foreach (var node in input.Nodes)
        {
            var graphNode = new Node(node.Name, node.Kind)
            {
                Width = (node.Raising.Count() + node.Listening.Count()) * .2f
            };

            digraph.AddNode(graphNode);

            foreach (var r in node.Raising)
            {
                var graphChildNode = new Node(r.Name, r.Kind, true);
                graphNode.AddChild(graphChildNode);
                digraph.AddNode(graphChildNode);
            }

            foreach (var l in node.Listening)
            {
                var graphChildNode = new Node(l.Name, l.Kind, false);

                digraph.AddNode(graphChildNode);
                graphChildNode.AddChild(graphNode);
            }
        }

        var svg = GetChartSvg(digraph);        
        AddMetadata(svg, input.Metadata);

        var filename = $"{Path.GetTempFileName()}.svg";
        File.WriteAllText(filename, svg.GetXML());

        return new ChartResult(filename);
    }

    private static void AddMetadata(SvgDocument svg, IDictionary<string, string> metadata)
    {
        foreach (var m in metadata.Reverse())
        {
            svg.Children.Insert(0, new SvgDocumentMetadata
            {
                ID = m.Key,
                Content = m.Value
            });
        }
    }

    private SvgDocument GetChartSvg(Digraph digraph)
    {
        using var client = new HttpClient();
        var output = digraph.Render();
        var content = new StringContent(output.Data, Encoding.UTF8, "application/json");
        var response = Task.Run(() => client.PostAsync(output.Url, content)).Result;        
        var stream = response.Content.ReadAsStream();
        
        return SvgDocument.Open<SvgDocument>(stream);
    }
}
