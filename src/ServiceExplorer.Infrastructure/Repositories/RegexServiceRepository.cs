using Humanizer;
using ServiceExplorer.Domain;
using System.Text.RegularExpressions;

namespace ServiceExplorer.Infrastructure.Repositories;

public class RegexServiceRepository : IServiceRepository
{
    private static readonly Regex _findListeningEventRegex = new(
        @"'*eventtype'*\s*:\s*'(?<serviceEvent>[a-z]+)'", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex _findRaisingIntegrationEventRegex = new(
        @"new (?<serviceEvent>[a-z]+IntegrationEvent)\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex _findRaisingSyncEventRegex = new(
        @"PersistEventAsync\s*\(\s*new (?<serviceEvent>[a-z]+SyncEvent)\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly string[] _folders;

    public RegexServiceRepository(params string[] folders)
    {
        _folders = folders;
    }

    public IEnumerable<Service> FindAll()
    {
        var services = new List<Service>();

        foreach (var folder in _folders)
        {
            var folderInfo = new DirectoryInfo(folder);

            var iaacFolders = folderInfo
                .GetDirectories("*.Iaac", SearchOption.AllDirectories)
                .Select(f => f.FullName)
                .ToArray();

            var selectedFolders = new List<string>();
            selectedFolders.AddRange(iaacFolders);

            AddServiceAndListeningEvents(services, selectedFolders);
            AddRaisingEvents(services, folder);
        }

        return services.OrderBy(s => s.Name);
    }

    private void AddRaisingEvents(List<Service> services, string folder)
    {
        // Adds the raising integration events.
        var domainFolders = new DirectoryInfo(folder)
            .GetDirectories("*", SearchOption.AllDirectories)
            .Where(f => !f.Name.Contains("Test"))
            .Select(f => f.FullName);


        foreach (var domainFolder in domainFolders)
        {
            var serviceName = GetServiceNameFromFolder(domainFolder);
            var service = services.SingleOrDefault(s => s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

            if (service is null)
                continue;

            AddRaising(domainFolder, service);
        }
    }

    private void AddRaising(string? folder, Service service)
    {
        var domainFiles = Directory
                            .GetFiles(folder!, "*.cs", SearchOption.AllDirectories);

        foreach (var domainFile in domainFiles)
        {
            // Integration events.
            var matches = _findRaisingIntegrationEventRegex.Matches(File.ReadAllText(domainFile));

            foreach (Match m in matches)
                service.AddRaising(CreateServiceEvent(m.Groups["serviceEvent"].Value));

            // Sync events.
            matches = _findRaisingSyncEventRegex.Matches(File.ReadAllText(domainFile));

            foreach (Match m in matches)
                service.AddRaising(CreateServiceEvent(m.Groups["serviceEvent"].Value));
        }
    }

    private void AddServiceAndListeningEvents(List<Service> services, IEnumerable<string> folders)
    {
        // Adds the service and the listening integration events.
        foreach (var folder in folders)
        {
            var service = Service.Registry.Get(GetServiceNameFromFolder(folder));
            AddListening(folder, service);

            services.Add(service);
        }
    }

    private void AddListening(string? folder, Service service)
    {
        AddListening(_findListeningEventRegex, folder, "*ServiceBusSubscriptions*.bicep", service);

        void AddListening(Regex regex, string? folder, string fileSearchPattern, Service service)
        {
            var targetFiles = Directory.GetFiles(folder!, fileSearchPattern, SearchOption.AllDirectories);

            foreach (var targetFile in targetFiles)
            {
                var matches = regex.Matches(File.ReadAllText(targetFile));

                foreach (Match m in matches)
                    service.AddListening(CreateServiceEvent(m.Groups["serviceEvent"].Value));
            }
        }
    }

    private ServiceEvent CreateServiceEvent(string rawName)
    {
        ArgumentNullException.ThrowIfNull(rawName, nameof(rawName));

        (var name, var kind) = ParseEventNameAndKind(rawName);

        return ServiceEvent.Registry.Get(name, kind);
    }

    public static (string, ServiceEventKind) ParseEventNameAndKind(string serviceEventRawName)
    {
        string name;
        ServiceEventKind kind;

        if (serviceEventRawName.EndsWith("SyncEvent"))
        {
            name = serviceEventRawName.Replace("SyncEvent", string.Empty, StringComparison.OrdinalIgnoreCase);
            kind = ServiceEventKind.Sync;
        }
        else
        {
            name = serviceEventRawName.Replace("IntegrationEvent", string.Empty, StringComparison.OrdinalIgnoreCase);
            kind = ServiceEventKind.Integration;
        }

        return (name, kind);
    }

    private static string GetServiceNameFromFolder(string folder)
    {
        if (folder.Contains("Mapiq.Databricks"))
            return "Data Science";
        else
            return Path.GetFileNameWithoutExtension(folder)
                .Replace("Mapiq.", "")
                .Replace("Service", "", StringComparison.OrdinalIgnoreCase)
                .Humanize(LetterCasing.Title)
                .Trim();

    }
}