namespace Dispatcher
{
    public interface IRequestServiceFactory
    {
        object Resolve<TResponse>(IRequest<TResponse> request);
        object Resolve(IRequest request);
    }
}
