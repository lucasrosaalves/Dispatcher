using System.Threading;
using System.Threading.Tasks;

namespace Dispatcher
{
    public interface IRequestService<in Tin, Tout> where Tin : IRequest<Tout>
    {
        Task<Tout> HandleAsync(Tin request, CancellationToken cancellationToken = default);
    }

    public interface IRequestService<in Tin> where Tin : IRequest
    {
        Task HandleAsync(Tin request, CancellationToken cancellationToken = default);
    }
}
