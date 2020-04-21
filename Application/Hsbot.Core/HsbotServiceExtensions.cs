using System;
using System.Linq;
using System.Reflection;
using Hsbot.Core.ApiClients;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.Connection;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Maps;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Hsbot.Core
{
    public static class HsbotServiceExtensions
    {
        public static HsbotServiceConfigurator AddHsbot(this IServiceCollection services, HsbotConfig config)
        {
            RegisterMessageHandlers(services);
            RegisterBotServices(services);

            services.AddSingleton<IHsbotConfig>(svc => config);
            services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
            services.AddSingleton<IBotBrainSerializer<HsbotBrain>, JsonBrainSerializer>();
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddSingleton<IMapProvider, GoogleMapProvider>();
            services.AddHttpClient<ITumblrApiClient, TumblrApiClient>();
            services.AddSingleton<Hsbot>();

            return new HsbotServiceConfigurator(services);
        }

        private static void RegisterMessageHandlers(IServiceCollection services)
        {
            RegisterImplementationsOfInterface<IInboundMessageHandler>(services, ServiceLifetime.Singleton);
        }

        private static void RegisterBotServices(IServiceCollection services)
        {
            RegisterImplementationsOfInterface<IBotService>(services, ServiceLifetime.Singleton);
        }

        private static void RegisterImplementationsOfInterface<T>(IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            var handlerInterfaceType = typeof(T);
            if (!handlerInterfaceType.IsInterface) throw new InvalidOperationException("This method is to be used for interface types only");

            var messageHandlerTypes = Assembly.GetAssembly(typeof(Hsbot))
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && handlerInterfaceType.IsAssignableFrom(t));

            foreach (var messageHandlerType in messageHandlerTypes)
            {
                services.Add(new ServiceDescriptor(handlerInterfaceType, messageHandlerType, serviceLifetime));
            }
        }
    }

    public class HsbotServiceConfigurator
    {
        private readonly IServiceCollection _serviceCollection;

        internal HsbotServiceConfigurator(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public HsbotServiceConfigurator AddChatServices<TConnector, TMessageFormatter>()
            where TConnector : class, IHsbotChatConnector
            where TMessageFormatter : class, IChatMessageTextFormatter
        {
            _serviceCollection.AddSingleton<IHsbotChatConnector, TConnector>();
            _serviceCollection.AddSingleton<IChatMessageTextFormatter, TMessageFormatter>();

            return this;
        }

        public HsbotServiceConfigurator AddBrainStorageProvider<T>()
            where T : class, IBotBrainStorage<HsbotBrain>
        {
            _serviceCollection.AddSingleton<IBotBrainStorage<HsbotBrain>, T>();
            return this;
        }
    }
}
