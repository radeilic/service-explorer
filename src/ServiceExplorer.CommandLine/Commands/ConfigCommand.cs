using ServiceExplorer.CommandLine.Configuration;
using ServiceExplorer.CommandLine.IO;
using McMaster.Extensions.CommandLineUtils;

namespace ServiceExplorer.CommandLine.Commands;

[Command(Name = "config", Description = "Set the folders where Mapiq-One repository is cloned.")]
public class ConfigCommand
{
    private readonly IConfig _config;
    private readonly IConfigWriter _configWriter;
    private readonly IFileSystem _fileSystem;
    private readonly IOutput _output;

    public ConfigCommand(IConfig config, IConfigWriter configWriter, IFileSystem fileSystem, IOutput output)
    {
        _config = config;
        _configWriter = configWriter;
        _fileSystem = fileSystem;
        _output = output;
    }

    [Option(
    ShortName = "af",
    LongName = "add-folder",
    Description = "Adds a folder where the Mapiq-One repository is cloned.")]
    public string? AddFolder { get; set; }

    [Option(
     ShortName = "rf",
     LongName = "remove-folder",
     Description = "Removes a folder where the Mapiq-One repository is cloned.")]
    public string? RemoveFolder { get; set; }

    public int OnExecute(CommandLineApplication app)
    {
        if (AddFolder is null && RemoveFolder is null)
        {
            app.ShowHelp();
        }
        else if (!string.IsNullOrEmpty(AddFolder))
        {
            if (_fileSystem.ExistsDirectory(AddFolder))
            {
                _configWriter.AddFolder(AddFolder);
                _output.WriteLine("Folder added.");
            }
            else
            {
                _output.WriteLine($"The folder '{AddFolder}' does not exist.");
                return -1;
            }

        }
        else if (!string.IsNullOrEmpty(RemoveFolder))
        {
            _configWriter.RemoveFolder(RemoveFolder);
            _output.WriteLine("Folder removed.");
        }

        _output.WriteFolders(_config);

        return 0;
    }   
}
