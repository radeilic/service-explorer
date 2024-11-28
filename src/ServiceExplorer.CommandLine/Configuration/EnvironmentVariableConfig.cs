namespace ServiceExplorer.CommandLine.Configuration;

public class EnvironmentVariableConfig : IConfig, IConfigWriter
{
    public EnvironmentVariableConfig(string? keySuffix = null)
    {
        FoldersVariableName = $"MAPIQ_SERVICE_EXPLORER_CONFIG_FOLDERS{keySuffix}";
    }

    private string FoldersVariableName;

    public string[] Folders
    {
        get => Environment
            .GetEnvironmentVariable(FoldersVariableName, EnvironmentVariableTarget.User)
            ?.Split(";", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    }

    public void AddFolder(string folder) => UpdateFolders(folders => folders.Add(folder));

    public void RemoveFolder(string folder) => UpdateFolders(folders => folders.Remove(folder));

    public void Reset() => UpdateFolders(folders => folders.Clear());

    void UpdateFolders(Action<IList<string>> update)
    {
        var foldersToUpdate = new List<string>(Folders);
        update(foldersToUpdate);

        Environment.SetEnvironmentVariable(FoldersVariableName, string.Join(';', foldersToUpdate.ToArray()), EnvironmentVariableTarget.User);
    }        
}
