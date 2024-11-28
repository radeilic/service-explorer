using ServiceExplorer.Domain;

namespace ServiceExplorer.Infrastructure.Drawing
{
    public class ChartBuilderInput<TParentNode, TChildNode>
          where TParentNode : INodeInfo<TChildNode>
          where TChildNode : INodeInfo
    {
        public ChartBuilderInput(IEnumerable<TParentNode> nodes, IDictionary<string, string> metadata)
        {
            Nodes = nodes;
            Metadata = metadata;
        }

        public IEnumerable<TParentNode> Nodes { get; }
        public IDictionary<string, string>  Metadata { get; }
    }
}
