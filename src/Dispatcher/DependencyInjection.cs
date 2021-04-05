using System;
using System.Linq;
using System.Reflection;
using Dispatcher.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher
{

    public static class DependencyInjection
    {
        private static Type[] _genericServiceTypes = new[]
        {
                typeof(IRequestService<>),
                typeof(IRequestService<,>)
        };

        public static IServiceCollection AddDispatcher(this IServiceCollection services, Assembly assembly)
        {
            if(assembly is null) { throw new ArgumentNullException(nameof(assembly)); }

            foreach (var genericServiceTypes in _genericServiceTypes)
            {
                services.RegisterGenericTypes(assembly, genericServiceTypes);
            }

            services.AddScoped<IRequestServiceFactory, RequestServiceFactory>();
            services.AddScoped<IDispatcherHandler, DispatcherHandler>();

            return services;
        }

        private static IServiceCollection RegisterGenericTypes(this IServiceCollection services,
            Assembly assembly, Type genericServiceTypes)
        {
            foreach (var concreteServiceType in assembly.GetTypes())
            {
                var abstractionServiceType = GetAbstractionType(concreteServiceType, genericServiceTypes);

                if (abstractionServiceType is null) { continue; }

                services.AddScoped(abstractionServiceType, concreteServiceType);
            }

            return services;
        }

        private static Type GetAbstractionType(Type concreteType, Type genericServiceType)
        {
            return
                concreteType?.GetInterfaces()?
                .FirstOrDefault(i =>
                {
                    return i.IsGenericType && genericServiceType == i.GetGenericTypeDefinition();
                });
        }
    }
}
