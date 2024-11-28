using System.Collections.Immutable;
using System.Diagnostics;

namespace ServiceExplorer.Domain;

[DebuggerDisplay("{Name}")]
public sealed class Service : IEquatable<Service>, INodeInfo<ServiceEvent>
{
    private IList<ServiceEvent> _listening;
    private IList<ServiceEvent> _raising;

    public string Name { get; }

    private Service(string name)
    {
        Name = name;
        _listening = new List<ServiceEvent>();
        _raising = new List<ServiceEvent>();
    }

    public IEnumerable<ServiceEvent> Listening => _listening.OrderBy(l => l.Name).ToImmutableArray();
    public IEnumerable<ServiceEvent> Raising => _raising.OrderBy(l => l.Name).ToImmutableArray();
    
    NodeKind INodeInfo.Kind => NodeKind.Service;

    public void AddListening(ServiceEvent @event)
    {
        if (!_listening.Any(l => l == @event))
        {
            _listening.Add(@event);
            @event.AddListening(this);
        }

    }

    public void AddRaising(ServiceEvent @event)
    {
        if (!_raising.Any(l => l == @event))
        {
            _raising.Add(@event);
            @event.AddRaising(this);
        }
    }

    public bool Equals(Service? other)
    {
        return other is not null
            && other.Name == Name;
    }

    public override bool Equals(object? obj) => Equals(obj as Service);

    public override int GetHashCode() => Name.GetHashCode();

    public void RemoveEvents(IEnumerable<ServiceEvent> events)
    {
        var eventsToRemove = events.ToArray();
        _listening = _listening.Where(l => !eventsToRemove.Contains(l)).ToList();
        _raising = _raising.Where(r => !eventsToRemove.Contains(r)).ToList();
    }
 
    public void RemoveEventsNotInContext(IEnumerable<ServiceEvent> events)
    {
        _listening = _listening.Where(l => events.Contains(l)).ToList();
        _raising = _raising.Where(r => events.Contains(r)).ToList();
    }

    public void ApplyFilter(ServiceFilter filter)
    {
        _listening = _listening.Where(e => FilterListening(e, filter)).ToList();
        _raising = _raising.Where(e => FilterRaising(e, filter)).ToList();
    }

    public static bool operator ==(Service left, Service right)
    {
        return EqualityComparer<Service>.Default.Equals(left, right);
    }

    public static bool operator !=(Service left, Service right)
    {
        return !(left == right);
    }

    private static bool FilterListening(ServiceEvent serviceEvent, IListeningEventFilter filter)
    {
        if (!filter.ShowListeningEvent)
            return false;

        return filter.ListeningEventName == null || serviceEvent.Name.Contains(filter.ListeningEventName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool FilterRaising(ServiceEvent serviceEvent, IRaisingEventFilter filter)
    {
        if (!filter.ShowRaisingEvents)
            return false;

        return filter.RaisingEventName == null || serviceEvent.Name.Contains(filter.RaisingEventName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Registry pattern: https://martinfowler.com/eaaCatalog/registry.html
    /// </summary>
    public static class Registry
    {
        private static Dictionary<string, Service> _instances = new Dictionary<string, Service>();

        public static Service Get(string name)
        {
            if (!_instances.ContainsKey(name))
                _instances.Add(name, new Service(name));

            return _instances[name];
        }

        public static void Reset() => _instances.Clear();            
    }              
}
