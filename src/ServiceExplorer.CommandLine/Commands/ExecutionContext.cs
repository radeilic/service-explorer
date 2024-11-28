using ServiceExplorer.Domain;

namespace ServiceExplorer.CommandLine.Commands;

public sealed class ExecutionContext
{
    public ExecutionContext(IServiceRepository serviceRepository)
    {        
        Explorer = new Explorer(serviceRepository);
    }
    
    public Explorer Explorer { get; }
}