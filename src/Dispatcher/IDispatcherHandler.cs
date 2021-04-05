using System.Threading;
using System.Threading.Tasks;

namespace Dispatcher
{
    public interface IDispatcherHandler
    {
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task SendAsync(IRequest request, CancellationToken cancellationToken = default);
    }
}
