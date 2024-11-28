using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;

namespace ServiceExplorer.Domain;

[DisplayName("Event")]
[DebuggerDisplay("{Name} ({Kind})")]
public class ServiceEvent : IEquatable<ServiceEvent>, INodeInfo<Service>
{
    private IList<Service> _listening;
    private IList<Service> _raising;

    private ServiceEvent(string name, ServiceEventKind kind)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        Name = name;
        Kind = kind;

        _listening = new List<Service>();
        _raising = new List<Service>();
    }

    public string Name { get; }
    public ServiceEventKind Kind { get; }

    public IEnumerable<Service> Listening => _listening.OrderBy(l => l.Name).ToImmutableArray();
    public IEnumerable<Service> Raising => _raising.OrderBy(l => l.Name).ToImmutableArray();

    NodeKind INodeInfo.Kind => NodeKind.Event;

    public void AddListening(Service service)
    {
        if (!_listening.Any(l => l == service))
            _listening.Add(service);
    }

    public void AddRaising(Service service)
    {
        if (!_raising.Any(l => l == service))
            _raising.Add(service);
    }

    public bool Equals(ServiceEvent? other)
    {
        return other is not null
            && other.Name == Name
            && other.Kind == Kind;
    }

    public override bool Equals(object? obj) => Equals(obj as ServiceEvent);

    public override int GetHashCode() => Name.GetHashCode();

    public void RemoveServicesNotInContext(IEnumerable<Service> services)
    {
        _listening = Listening.Where(l => services.Contains(l)).ToList();
        _raising = _raising.Where(l => services.Contains(l)).ToList();
    }
    public void ApplyFilter(EventFilter filter)
    {
        _listening = _listening.Where(s => FilterListening(s, filter)).ToList();
        _raising = _raising.Where(s => FilterRaising(s, filter)).ToList();
    }

    public static bool operator ==(ServiceEvent left, ServiceEvent right)
    {
        return EqualityComparer<ServiceEvent>.Default.Equals(left, right);
    }

    public static bool operator !=(ServiceEvent left, ServiceEvent right)
    {
        return !(left == right);
    }

    private bool FilterRaising(Service service, EventFilter filter) => filter.ShowServiceRaising;
    private bool FilterListening(Service service, EventFilter filter) => filter.ShowServiceListening;        

    /// <summary>
    /// Registry pattern: https://martinfowler.com/eaaCatalog/registry.html
    /// </summary>
    public static class Registry
    {
        private static Dictionary<string, ServiceEvent> _instances = new Dictionary<string, ServiceEvent>();

        public static ServiceEvent Get(string name, ServiceEventKind kind)
        {
            var key = $"{name}_{kind}";

            if (!_instances.ContainsKey(key))
                _instances.Add(key, new ServiceEvent(name, kind));

            return _instances[key];
        }

        public static void Reset() => _instances.Clear();
    }
}