namespace ServiceExplorer.Domain;

public static class ServiceEventExtensions
{
    public static bool IsOnlySelfListened(this ServiceEvent @event)
    {
        return @event.Raising.Any(r =>
            @event.Listening.Count() == 1
            && @event.Listening.Contains(r)
            && @event.Raising.Count() == 1
            && @event.Raising.Contains(r));
    }
}
