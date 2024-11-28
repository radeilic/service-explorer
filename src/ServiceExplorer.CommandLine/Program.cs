using ServiceExplorer.CommandLine.Commands;
using ServiceExplorer.CommandLine.Configuration;
using ServiceExplorer.CommandLine.IO;
using ServiceExplorer.Domain;
using ServiceExplorer.Infrastructure.Drawing;
using ServiceExplorer.Infrastructure.Drawing.QuickChart;
using ServiceExplorer.Infrastructure.Repositories;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceExplorer.CommandLine;

[Subcommand(typeof(ServiceCommand))]
[Subcommand(typeof(EventCommand))]
[Subcommand(typeof(ConfigCommand))]
public class Program
{
    private const string AppName = "ServiceExplorer";
    public static string CurrentRawCommand { get; private set; } = string.Empty;

    public static int Main(string[] args)
    {
        var app = CreateApp();

        try
        {
            CurrentRawCommand = $"{AppName} {string.Join(" ", args)}";
            return app.Execute(args);
        }
        catch (UnrecognizedCommandParsingException ex)
        {
            Console.WriteLine(ex.Message);

            if (ex.NearestMatches.Any())
            {
                Console.WriteLine();
                Console.WriteLine("The most similar command are:");

                foreach (var suggestion in ex.NearestMatches)
                    Console.WriteLine($"\t{suggestion}");
            }

            return -1;
        }
        catch (CommandParsingException ex)
        {
            Console.WriteLine(ex.Message);
            return -1;
        }
    }
    
    private static CommandLineApplication<Program> CreateApp()
    {
        var services = new ServiceCollection()
               .AddSingleton<IConfig, EnvironmentVariableConfig>()
               .AddSingleton<IConfigWriter, EnvironmentVariableConfig>()
               .AddSingleton<IServiceRepository>(s => new RegexServiceRepository(s.GetRequiredService<IConfig>().Folders))
               .AddSingleton<IFileSystem, PhysicalFileSystem>()
               .AddSingleton<IOutput, PhysicalConsoleOutput>()
               .AddSingleton<IChartBuilder, QuickChartBuilder>()
               .AddSingleton(PhysicalConsole.Singleton)
               .BuildServiceProvider();

        var app = new CommandLineApplication<Program>();
        app.Conventions            
            .UseDefaultConventions()
            .UseConstructorInjection(services);

        app.Name = AppName;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        app.VersionOption("--version", typeof(Program).Assembly.GetName().Version.ToString());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        app.HelpTextGenerator = new CustomHelpTextGenerator();              
        return app;
    }
}