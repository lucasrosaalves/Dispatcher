using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatcher.Internal
{
    internal class DispatcherHandler : IDispatcherHandler
    {
        private readonly IRequestServiceFactory _serviceFactory;

        public DispatcherHandler(IRequestServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory
                ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var service = _serviceFactory.Resolve(request);

            return await InvokeHandler<Task<TResponse>, TResponse>(service, request, cancellationToken).ConfigureAwait(false);
        }

        public async Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var service = _serviceFactory.Resolve(request);

            await InvokeHandler<Task>(service, request, cancellationToken).ConfigureAwait(false);
        }

        private static T InvokeHandler<T,TResponse>(object service, IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var parametersArray = new object[]
            {
                request,
                cancellationToken
            };

            return InvokeHandler<T>(service, parametersArray);
        }

        private static T InvokeHandler<T>(object service, IRequest request, CancellationToken cancellationToken)
        {
            var parametersArray = new object[]
            {
                request,
                cancellationToken
            };

            return InvokeHandler<T>(service, parametersArray);
        }

        private static T InvokeHandler<T>(object service, object[] parametersArray)
        {
            return (T)GetHandler(service).Invoke(service, parametersArray);
        }

        private static MethodInfo GetHandler(object service)
        {
            var handler = service
                .GetType()?
                .GetMethods()?
                .FirstOrDefault(m => string.Compare(m.Name, "HandleAsync", true) == 0);

            if(handler is null)
            {
                throw new ArgumentException($"Could not find a handler for {service.GetType().Name}");
            }

            return handler;
        }
    }
}
