
namespace ApplicationTracker.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProcessDataRepository ProcessTable { get; }
        int Complete();
    }
}
