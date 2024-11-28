using ServiceExplorer.Domain;
using System.Text;

namespace ServiceExplorer.Infrastructure.Drawing.QuickChart;

    /// <summary>
    /// Morea at  https://quickchart.io/documentation/graphviz-api/.
    /// </summary>
    class Digraph
    {
        private List<Node> _nodes = new List<Node>();

        public void AddNode(Node node)
        {
            _nodes.Add(node);
        }

        public RenderOutput Render()
        {
            var data = new StringBuilder();

            Append(data, " { \n");
            Append(data, "\"graph\": \"digraph {");

            if (_nodes.Any())
            {
                var previousNodeKind = _nodes.First().Kind;
                var drivenByService = previousNodeKind == NodeKind.Service;

                Append(data, "\\nedge[color = blue]\\n");

                foreach (var node in _nodes)
                {
                    if (previousNodeKind != node.Kind)
                    {
                        if (node.Kind == NodeKind.Service)
                            Append(data, "\\nedge[color = blue]\\n");
                        else
                            Append(data, "\\nedge[color = red]\\n");

                        previousNodeKind = node.Kind;
                    }

                    if (drivenByService)
                        Append(data, $"{node.RenderName()} -> {{ {string.Join(",", node.Children.Select(c => c.RenderName()))} }}\\n");
                    else
                    {
                        foreach (var child in node.Children)
                        {
                            if (child.Raising.HasValue && child.Raising.Value)
                                Append(data, "\\nedge[color = blue]\\n");
                            else
                                Append(data, "\\nedge[color = red]\\n");

                            Append(data, $"{child.RenderName()} -> {{ {node.RenderName()} }}\\n");
                        }
                    }
                }

                Append(data, "\\n");

                foreach (var node in _nodes)
                    Append(data, node.Render());
            }

            Append(data, "}\" }");

            return new RenderOutput("https://quickchart.io/graphviz", data.ToString());
        }


        private void Append(StringBuilder output, string value) => output.Append(value);
    }
