namespace ServiceExplorer.Domain;

public interface IRaisingEventFilter
{
    string? RaisingEventName { get; init; }
    bool ShowRaisingEvents { get; init; }
}