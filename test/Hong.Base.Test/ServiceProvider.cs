using Hong.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hong.Test
{
    public class ServiceProvider
    {
        public static IServiceProvider CreateServiceProvider(Action<IServiceCollection> init)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHong();

            init(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }
    }
}
