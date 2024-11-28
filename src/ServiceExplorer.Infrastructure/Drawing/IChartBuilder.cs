using ServiceExplorer.Domain;

namespace ServiceExplorer.Infrastructure.Drawing;

public interface IChartBuilder
{
    ChartResult Build<TParentNode, TChildNode>(ChartBuilderInput<TParentNode, TChildNode> input)
        where TParentNode : INodeInfo<TChildNode>
        where TChildNode : INodeInfo;
}