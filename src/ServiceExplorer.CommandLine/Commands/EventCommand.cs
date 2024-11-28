using ServiceExplorer.CommandLine.Configuration;
using ServiceExplorer.CommandLine.IO;
using ServiceExplorer.Domain;
using ServiceExplorer.Infrastructure.Drawing;
using McMaster.Extensions.CommandLineUtils;

namespace ServiceExplorer.CommandLine.Commands;

[Command(Name = "event", Description = "Explore integration events, what microservices are raising and listening them.")]
public sealed class EventCommand : CommandBase
{
    public EventCommand(
        IConfig config,
        IServiceRepository serviceRepository,
        IFileSystem fileSystem,
        IOutput output,
        IChartBuilder chartBuilder)
        : base(config, serviceRepository, fileSystem, output, chartBuilder)
    {
    }

    [Option(
        ShortName = "fe",
        LongName = "filter-event",
        Description = "Filter event by name.")]
    public string[]? Filter { get; set; }

    [Option(
     ShortName = "inle",
     LongName = "ignore-not-listened-event",
     Description = "Ignore events that are raised by not listened.")]
    public bool IgnoreNotListenedEvent { get; set; }

    [Option(
        ShortName = "ssr",
        LongName = "show-service-raising",
        Description = "If should show services that are raising the events in the result output.")]
    public bool ShowServiceRaising { get; set; }

    [Option(
        ShortName = "ssl",
        LongName = "show-service-listening",
        Description = "If should show services that are listening the events in the result output.")]
    public bool ShowServiceListening { get; set; }

    [Option(
       ShortName = "p",
       LongName = "plot",
       Description = "If should plot a chart as the ouput")]
    public bool IsChartOutput { get; set; }

    protected override void Execute(ExecutionContext context)
    {
        var filter = new EventFilter
        {
            Names = Filter ?? Array.Empty<string>(),
            IgnoreNotListenedEvent = IgnoreNotListenedEvent,
            ShowServiceRaising = ShowServiceRaising,
            ShowServiceListening = ShowServiceListening,
        };

        if (IsChartOutput)
        {
            WriteResults(
                 () => context.Explorer.GetEvents(filter),
                 events =>
                 {
                     WriteInfo("Plotting...");
                     ChartBuilder.Build(events, Program.CurrentRawCommand).Show();
                 });
        }
        else
        {
            WriteResults(
                () => context.Explorer.GetEvents(filter),
                e =>
                {
                    WriteSubtitle($"{e.Name} ({e.Kind})");
                    ListServices("Raised by", e.Raising);
                    ListServices("Listened by", e.Listening);
                });
        }
    }

    private void ListServices(string kind, IEnumerable<Service> services) => WriteSubResults(kind, services, s => s.Name);
}
