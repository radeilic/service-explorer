using Humanizer;
using ServiceExplorer.CommandLine.Configuration;
using ServiceExplorer.CommandLine.IO;
using ServiceExplorer.Domain;
using ServiceExplorer.Infrastructure.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace ServiceExplorer.CommandLine.Commands;

public abstract class CommandBase
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IFileSystem _fileSystem;
    private readonly IOutput _output;    

    protected CommandBase(
        IConfig config,
        IServiceRepository serviceRepository,
        IFileSystem fileSystem,
        IOutput output,
        IChartBuilder chartBuilder)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(serviceRepository);
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(output);

        Config = config;
        _serviceRepository = serviceRepository;
        _fileSystem = fileSystem;
        _output = output;
        ChartBuilder = chartBuilder;
    }

    protected IConfig Config { get; }
    protected IChartBuilder ChartBuilder { get; }

    public int OnExecute()
    {                
        WriteTitle("Service Explorer");

        if (Config.Folders.Length < 1)
        {
            WriteError(@"Before start, you should define the Mapiq-One folder. 
                        Use the command: config --add-folder path/to/folder");

            _output.WriteFolders(Config);
            return -1;
        }

        foreach (var folder in Config.Folders)
        {
            if (!_fileSystem.ExistsDirectory(folder))
            {
                WriteError($"Folder '{folder}' does not exist.");
                return -1;
            }            
        }

        try
        {
            _output.WriteFolders(Config);
            Execute(new ExecutionContext(_serviceRepository));
        }
        catch(Exception ex)
        {
            WriteError($"Unknown error: {ex.Message}");
            return -1;
        }

        return 0;
    }

    protected abstract void Execute(ExecutionContext context);    

    protected void WriteResults<TResult>(Func<IEnumerable<TResult>> getResults, Action<TResult[]> showResults)
    {
        var resultName = GetName<TResult>();
        WriteInfo($"\nGetting {resultName} information...");
        var results = getResults().ToArray();

        WriteInfo($"{resultName.Humanize()} found: {results.Length}", false);
        NewLine();

        showResults(results);

        WriteInfo("Done.");
    }
    
    protected void WriteResults<TResult>(Func<IEnumerable<TResult>> getResults, Action<TResult> showResult)
    {
        WriteResults(
            getResults,
            results =>
            {
                for (int i = 0; i < results.Length; i++)
                {
                    showResult(results[i]);

                    // Is in the bottom of the window?
                    if (i < results.Length - 1 && _output.LinePosition == _output.TotalLinesAvailable - 1)
                    {
                        // Yes, so should page the result.
                        _output.WriteLine("-- More --");

                        switch (_output.ReadKey().Key)
                        {
                            case ConsoleKey.Q:
                                return;

                            default:
                                _output.LinePosition = _output.TotalLinesAvailable - 2;
                                _output.WriteLine(new string(' ', _output.TotalColumnsAvailable));
                                _output.LinePosition = _output.TotalLinesAvailable - 2;
                                continue;
                        }
                    }
                }
            });
    }

    protected void WriteSubResults<TSubResult>(string kind, IEnumerable<TSubResult> subResults, Func<TSubResult, string> getSubResultMessage)
    {
        if (!subResults.Any())
            return;

        var name = GetName<TSubResult>();
        WriteInfo($"\t{kind} {subResults.Count()} {name}");

        foreach (var s in subResults)
        {
            WriteInfo($"\t\t* {getSubResultMessage(s)}");
        }
    }
    
    protected void WriteSubtitle(string msg)
    {
        NewLine();
        Write(msg, ConsoleColor.Green);
    }

    protected void WriteInfo(string msg, bool newLine = true) => Write(msg, ConsoleColor.White, newLine);

    private void WriteTitle(string msg) => Write(msg.ToUpperInvariant(), ConsoleColor.Blue);        
    private void WriteError(string msg) => Write(msg, ConsoleColor.Red);

    private void NewLine() => Write(string.Empty, ConsoleColor.White);

    private void Write(string msg, ConsoleColor color, bool newLine = true)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;

        if(newLine)
            _output.WriteLine(msg);
        else
            _output.Write(msg);

        Console.ForegroundColor = previousColor;
    }

    private string GetName<T>()
    {
        var type = typeof(T);
        var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
        var name = displayNameAttr == null ? type.Name : displayNameAttr.DisplayName;

        return $"{name.ToLowerInvariant()}s";
    }
}
