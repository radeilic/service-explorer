namespace ServiceExplorer.Domain;

public sealed class EventFilter
{
    public string[]? Names { get; init; }
    public bool ShowServiceRaising { get; init; }
    public bool ShowServiceListening { get; init; }
    public bool IgnoreNotListenedEvent { get; init; }        
}

