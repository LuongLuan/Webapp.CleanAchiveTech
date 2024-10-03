

namespace Application.Interface
{
    public interface IUnitOfWork : IDisposable
    { 
        Task<int> CommitAsync();
    }
}
