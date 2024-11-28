namespace ServiceExplorer.Domain;

public interface IListeningEventFilter
{
    string? ListeningEventName { get; init; }
    bool ShowListeningEvent { get; init; }
}