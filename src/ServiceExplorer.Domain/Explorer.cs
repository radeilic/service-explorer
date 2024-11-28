namespace ServiceExplorer.Domain;

public class Explorer
{
    private readonly IServiceRepository _serviceRepository;
    private IEnumerable<Service>? _servicesCache;

    public Explorer(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public IEnumerable<Service> GetAllServices() => _servicesCache ??= _serviceRepository.FindAll();

    public IEnumerable<Service> GetServices(ServiceFilter filter)
    {
        var services = GetAllServices()
            .Where(FilterByName)
            .Where(FilterByRaisingEvent)
            .Where(FilterByListeningEvent)
            .ToArray();

        foreach (var service in services)
            service.ApplyFilter(filter);

        RemoveServicesNotInContext(services);
        RemoveNotListenedEvents(filter.IgnoreNotListenedEvent, services);
        RemoveNotRaisedEvents(filter.IgnoreNotRaisedEvent, services);

        if (filter.IgnoreServiceWithoutEvent)
        {
            services = services.Where(s => s.Raising.Any() || s.Listening.Any()).ToArray();
        }

        return services;

        bool FilterByName(Service s)
        {
            return filter.Names == null
                || filter.Names.Length == 0
                || filter.Names.Any(name => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        bool FilterByRaisingEvent(Service s)
        {
            return filter.RaisingEventName == null
                || s.Raising.Any(e => e.Name.Contains(filter.RaisingEventName, StringComparison.OrdinalIgnoreCase));
        }

        bool FilterByListeningEvent(Service s)
        {
            return filter.ListeningEventName == null 
                || s.Listening.Any(e => e.Name.Contains(filter.ListeningEventName, StringComparison.OrdinalIgnoreCase));
        }
    }
    public IEnumerable<ServiceEvent> GetEvents(EventFilter filter)
    {
        var services = GetAllServices();
        var events = new List<ServiceEvent>(services.SelectMany(s => s.Raising));
        events.AddRange(services.SelectMany(s => s.Listening));

        var uniqueEvents = events
            .Where(FilterByName)
            .Distinct();

        foreach (var e in uniqueEvents)
            e.ApplyFilter(filter);

        if (filter.IgnoreNotListenedEvent)
            uniqueEvents = uniqueEvents.Where(e => e.Listening.Any());

        RemoveEventsNotInContext(uniqueEvents);

        return uniqueEvents;

        bool FilterByName(ServiceEvent e)
        {
            return filter.Names == null 
                || filter.Names.Length == 0 
                || filter.Names.Any(name => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void RemoveServicesNotInContext(IEnumerable<Service> services)
    {
        foreach (var service in services)
        {
            foreach(var @event in service.Listening)
                @event.RemoveServicesNotInContext(services);

            foreach (var @event in service.Raising)
                @event.RemoveServicesNotInContext(services);
        }
    }

    private void RemoveEventsNotInContext(IEnumerable<ServiceEvent> events)
    {
        foreach (var @event in events)
        {
            foreach (var service in @event.Listening)
                service.RemoveEventsNotInContext(events);

            foreach (var service in @event.Raising)
                service.RemoveEventsNotInContext(events);
        }
    }

    private static IEnumerable<ServiceEvent> RemoveNotListenedEvents(bool ignoreNotListenedEvents, IEnumerable<Service> services)
    {
        var eventsToRemove = new List<ServiceEvent>();

        if (ignoreNotListenedEvents)
        {
            var raisingEvents = services.SelectMany(s => s.Raising).ToArray();
            var listeninEvents = services.SelectMany(s => s.Listening).ToArray();
            var notListenedEvents = raisingEvents.Except(listeninEvents).ToArray();
            var selfListeningEvents = listeninEvents.Where(e => e.IsOnlySelfListened()).ToArray();                
            eventsToRemove.AddRange(notListenedEvents);
            eventsToRemove.AddRange(selfListeningEvents);

            foreach (var service in services)
                service.RemoveEvents(eventsToRemove);         

        }

        return eventsToRemove;
    }

    private static void RemoveNotRaisedEvents(bool ignoreNotRaisedEvents, IEnumerable<Service> services)
    {
        if (ignoreNotRaisedEvents)
        {
            var raisingEvents = services.SelectMany(s => s.Raising);
            var listeninEvents = services.SelectMany(s => s.Listening);
            var notRaisedEvents = listeninEvents.Except(raisingEvents);

            foreach (var service in services)
                service.RemoveEvents(notRaisedEvents);
        }
    }       
}
