namespace ServiceExplorer.Domain;

public interface IServiceRepository
{
    IEnumerable<Service> FindAll();
}
