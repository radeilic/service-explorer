using ServiceExplorer.CommandLine.Configuration;
using ServiceExplorer.CommandLine.IO;
using ServiceExplorer.Domain;
using ServiceExplorer.Infrastructure.Drawing;
using McMaster.Extensions.CommandLineUtils;

namespace ServiceExplorer.CommandLine.Commands;

[Command(Name = "service", Description = "Explore microservices, what integration events they are raising and listening to, and their relationship to other microservices.")]
public sealed class ServiceCommand : CommandBase
{
    public ServiceCommand(
        IConfig config,
        IServiceRepository serviceRepository,
        IFileSystem fileSystem,
        IOutput output,
        IChartBuilder chartBuilder)
        : base(config, serviceRepository, fileSystem, output, chartBuilder)
    {
    }

    [Option(
        ShortName = "fs",
        LongName = "filter-service",
        Description = "Filter services by name.")]
    public string[]? Filter { get; set; }

    [Option(
        ShortName = "iswe",
        LongName = "ignore-service-without-event",
        Description = "Ignore services without events.")]
    public bool IgnoreServiceWithoutEvent { get; set; }

    [Option(
       ShortName = "fre",
       LongName = "filter-raising-event",
       Description = "Filter services by raising event name.")]
    public string? RaisingEventName { get; set; }

    [Option(
     ShortName = "inre",
     LongName = "ignore-not-raised-event",
     Description = "Ignore events that are listened by not raised.")]
    public bool IgnoreNotRaisedEvent { get; set; }

    [Option(
        ShortName = "fle",
        LongName = "filter-listening-event",
        Description = "Filter services by listening event name.")]
    public string? ListeningEventName { get; set; }

    [Option(
      ShortName = "inle",
      LongName = "ignore-not-listened-event",
      Description = "Ignore events that are raised by not listened.")]
    public bool IgnoreNotListenedEvent { get; set; }

    [Option(
        ShortName = "sre",
        LongName = "show-raising-event",
        Description = "If should show raising events in the result output.")]
    public bool ShowRaisingEvent { get; set; }

    [Option(
        ShortName = "sle",
        LongName = "show-listening-event",
        Description = "If should show listening events in the result output.")]
    public bool ShowListeningEvent { get; set; }

    [Option(
       ShortName = "p",
       LongName = "plot",
       Description = "If should plot a chart as the ouput")]
    public bool IsChartOutput { get; set; }

    protected override void Execute(ExecutionContext context)
    {
        var filter = new ServiceFilter
        {
            Names = Filter ?? Array.Empty<string>(),
            ShowRaisingEvents = ShowRaisingEvent,
            RaisingEventName = RaisingEventName,
            IgnoreNotRaisedEvent = IgnoreNotRaisedEvent,
            ShowListeningEvent = ShowListeningEvent,
            ListeningEventName = ListeningEventName,
            IgnoreNotListenedEvent = IgnoreNotListenedEvent,
            IgnoreServiceWithoutEvent = IgnoreServiceWithoutEvent
        };

        if (IsChartOutput)
        {
            WriteResults(
                 () => context.Explorer.GetServices(filter),
                 services =>
                 {
                     base.WriteInfo("Plotting...");
                     ChartBuilder.Build(services, Program.CurrentRawCommand).Show();
                 });
        }
        else
        {
            WriteResults(
                () => context.Explorer.GetServices(filter),
                s =>
                {
                    WriteSubtitle(s.Name);                    
                    ListEvents("Raising", s.Raising);
                    ListEvents("Listening", s.Listening);
                });
        }
    }

    private void ListEvents(string kind, IEnumerable<ServiceEvent> serviceEvents) 
        => WriteSubResults(kind, serviceEvents, e => $"{e.Name} ({e.Kind})");
}
