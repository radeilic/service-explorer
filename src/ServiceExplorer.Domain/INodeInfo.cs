namespace ServiceExplorer.Domain;

public enum NodeKind
{
    Service,
    Event
}

public interface INodeInfo
{
    string Name { get; }
    NodeKind Kind { get; }
}

public interface INodeInfo<TChild> : INodeInfo
    where TChild : INodeInfo
{       
    IEnumerable<TChild> Listening { get; }
    IEnumerable<TChild> Raising { get; }
}

