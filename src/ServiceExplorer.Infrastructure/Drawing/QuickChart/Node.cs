using ServiceExplorer.Domain;
using ServiceExplorer.Infrastructure.Texts;
using System.Collections.Immutable;
using System.Text;

namespace ServiceExplorer.Infrastructure.Drawing.QuickChart;

class Node
{
    private List<Node> _children = new List<Node>();

    public Node(string name, NodeKind kind, bool? raising = null)
    {
        Name = name;
        Kind = kind;
        Raising = raising;
        FillColor = new string($"{kind}_{Name}".Reverse().ToArray()).ToHexColor();

        if (kind == NodeKind.Service)
            Shape = Shape.Circle;
        else
            Shape = Shape.Rectangle;
    }

    public string Name { get; }
    public NodeKind Kind { get; }
    public bool? Raising { get; }
    public Shape Shape { get; } = Shape.Square;
    public float Width { get; init; } = 1;
    public string Style { get; init; } = "filled,bold";
    public string Color { get; init; } = "white";
    public string FillColor { get; }
    public string FontName { get; init; } = "Inter";
    public string FontColor { get; init; } = "white";

    public IEnumerable<Node> Children => _children.ToImmutableArray();

    public void AddChild(Node child) => _children.Add(child);

    public string RenderName() => $"\\\"{Name.Replace(" ", "\\n")}\\\"";

    public string Render()
    {
        var builder = new StringBuilder();

        builder.Append(RenderName());
        builder.Append("[ ");
#pragma warning disable CA1308 // Normalize strings to uppercase
        builder.Append($"shape = {Shape.ToString().ToLowerInvariant()} ");
#pragma warning restore CA1308 // Normalize strings to uppercase
        builder.Append($"width = \\\"{Width}\\\" ");
        builder.Append($"style = \\\"{Style}\\\" ");
        builder.Append($"color = {Color} ");
        builder.Append($"fillcolor = \\\"{FillColor}\\\" ");
        builder.Append($"fontname = {FontName} ");
        builder.Append($"fontcolor = \\\"{FontColor}\\\" ");
        builder.Append("]\\n");

        return builder.ToString();
    }
}
