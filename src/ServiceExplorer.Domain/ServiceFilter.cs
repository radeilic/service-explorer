namespace ServiceExplorer.Domain;

public sealed class ServiceFilter : IRaisingEventFilter, IListeningEventFilter
{
    public string[] Names { get; init; } = Array.Empty<string>();
    public bool ShowRaisingEvents { get; init; }
    public string? RaisingEventName { get; init; }
    public bool ShowListeningEvent { get; init; }
    public string? ListeningEventName { get; init; }
    public bool IgnoreNotListenedEvent { get; init; }

    public bool IgnoreNotRaisedEvent { get; init; }
    public bool IgnoreServiceWithoutEvent { get; init; }
}

