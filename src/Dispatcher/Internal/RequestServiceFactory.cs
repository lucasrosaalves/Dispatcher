using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Internal
{
    internal class RequestServiceFactory : IRequestServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private ConcurrentDictionary<Type, Lazy<object>> _instances = new();

        public RequestServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object Resolve<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();

            var service = _instances.GetOrAdd(requestType, (requestType) =>
            {
                var serviceType = typeof(IRequestService<,>).MakeGenericType(requestType, typeof(TResponse));

                return new Lazy<object>(GetRequiredService(serviceType));

            }).Value;

            if (service is null) { throw new ArgumentException($"Could not find a service for {requestType.Name}"); }

            return service;
        }

        public object Resolve(IRequest request)
        {
            var requestType = request.GetType();

            var service = _instances.GetOrAdd(requestType, (requestType) =>
            {
                var serviceType = typeof(IRequestService<>).MakeGenericType(requestType);

                return new Lazy<object>(GetRequiredService(serviceType));

            }).Value;

            if (service is null) { throw new ArgumentException($"Could not find a service for {requestType.Name}"); }

            return service;
        }

        private object GetRequiredService(Type type)
        {
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetService(type);
        }
    }
}
