using System;
using System.Linq;
using System.Reflection;
using Hsbot.Core.ApiClients;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.Connection;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Maps;
using Hsbot.Core.MessageHandlers.Celebrations;
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
            RegisterBrainServices(services);
            RegisterReminderServices(services);
            RegisterMemoryServices(services);
            RegisterCelebrations(services);
            RegisterMessageHandlers(services);

            services.AddSingleton<IHsbotConfig>(svc => config);
            services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddSingleton<IMapProvider, GoogleMapProvider>();
            services.AddHttpClient<ITumblrApiClient, TumblrApiClient>();
            services.AddHttpClient<IJiraApiClient, JiraApiClient>();
            services.AddHttpClient<IXkcdApiClient, XkcdApiClient>();
            services.AddSingleton<Hsbot>();

            return new HsbotServiceConfigurator(services);
        }

        private static void RegisterReminderServices(IServiceCollection services)
        {
            services.AddSingleton<ReminderService>();
            services.AddSingleton<IReminderService, ReminderService>(x => x.GetRequiredService<ReminderService>());
            services.AddSingleton<IBotService, ReminderService>(x => x.GetRequiredService<ReminderService>());
        }

        private static void RegisterBrainServices(IServiceCollection services)
        {
            services.AddSingleton<IBotBrainSerializer<InMemoryBrain>, JsonBrainSerializer>();
            services.AddSingleton<HsbotBrainService>();
            services.AddSingleton<IBotBrain, HsbotBrainService>(x => x.GetRequiredService<HsbotBrainService>());
            services.AddSingleton<IBotService, HsbotBrainService>(x => x.GetRequiredService<HsbotBrainService>());
        }

        private static void RegisterMemoryServices(IServiceCollection services)
        {
            services.AddSingleton<MemoryService>();
            services.AddSingleton<IMemoryService, MemoryService>(x => x.GetRequiredService<MemoryService>());
            services.AddSingleton<IBotService, MemoryService>(x => x.GetRequiredService<MemoryService>());
        }

        private static void RegisterMessageHandlers(IServiceCollection services)
        {
            RegisterImplementationsOfInterface<IInboundMessageHandler>(services, ServiceLifetime.Singleton);
        }

        private static void RegisterCelebrations(IServiceCollection services)
        {
            RegisterImplementationsOfInterface<ICelebration>(services, ServiceLifetime.Singleton);
        }

        private static void RegisterImplementationsOfInterface<T>(IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            var handlerInterfaceType = typeof(T);
            if (!handlerInterfaceType.IsInterface) throw new InvalidOperationException("This method is to be used for interface types only");

            var implementationTypes = Assembly.GetAssembly(typeof(Hsbot))
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && handlerInterfaceType.IsAssignableFrom(t));

            foreach (var implementationType in implementationTypes)
            {
                services.Add(new ServiceDescriptor(handlerInterfaceType, implementationType, serviceLifetime));
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
            where T : class, IBotBrainStorage<InMemoryBrain>
        {
            _serviceCollection.AddSingleton<IBotBrainStorage<InMemoryBrain>, T>();
            return this;
        }

        public HsbotServiceConfigurator AddLogging<T>()
            where T : class, IHsbotLog
        {
            _serviceCollection.AddLogging();
            _serviceCollection.AddTransient<IHsbotLog, T>();
            return this;
        }
    }
}
