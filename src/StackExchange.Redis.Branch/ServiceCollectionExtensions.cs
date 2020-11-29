using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Branch.Repository;
using System;
using System.Reflection;

namespace StackExchange.Redis.Branch
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds redis repositories to DI as Singleton. Use this when you already add connection multiplexer to DI as Singleton.
        /// </summary>
        /// <param name="services">IServiceCollection instance.</param>
        /// <param name="assembly">Assembly which contains redis repositories.</param>
        /// <returns></returns>
        public static IServiceCollection AddRedisBranch(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            services.AddRedisBranches(assemblies);
            return services;
        }

        /// <summary>
        /// Helper method to add redis repositories to DI as Scoped.
        /// </summary>
        /// <param name="assemblies"></param>
        /// <remarks>
        ///     If any derived class from RedisRepositoryBase added to DI as Singleton, 
        ///     making it thread-safe is the developer's responsibility.
        /// </remarks> 
        private static void AddRedisBranches(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract &&
                        (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(RedisRepositoryBase<>)
                        || type.BaseType.BaseType != default && type.BaseType.BaseType.IsGenericType && type.BaseType.BaseType.GetGenericTypeDefinition() == typeof(RedisRepositoryBase<>)
                        ))
                    {
                        Type entityType = type.BaseType.GetGenericArguments()[0];

                        var iRepositoryType = typeof(IRedisRepository<>);
                        var iRepository = iRepositoryType.MakeGenericType(entityType);

                        var serviceDescriptor = new ServiceDescriptor(iRepository, type, ServiceLifetime.Scoped);
                        services.Add(serviceDescriptor);
                    }
                }
            }
        }
    }

}