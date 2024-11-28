using ServiceExplorer.Domain;

namespace ServiceExplorer.Infrastructure.Drawing;

public static class ChartBuilderExtensions
{
    public static ChartResult Build(this IChartBuilder builder, IEnumerable<Service> services, string rawCommand)
        => builder.Build(CreateInput<Service, ServiceEvent>(services, rawCommand));

    public static ChartResult Build(this IChartBuilder builder, IEnumerable<ServiceEvent> events, string rawCommand)
        => builder.Build(CreateInput<ServiceEvent, Service>(events, rawCommand));

    private static ChartBuilderInput<TParentNode, TChildNode> 
        CreateInput<TParentNode, TChildNode>(IEnumerable<TParentNode> nodes, string rawCommand)
          where TParentNode : INodeInfo<TChildNode>
          where TChildNode : INodeInfo
    {
        return new ChartBuilderInput<TParentNode, TChildNode>(
            nodes,
            new Dictionary<string, string>()
            {
                { "ServiceExplorer:Command", rawCommand }
            });
    }
}
