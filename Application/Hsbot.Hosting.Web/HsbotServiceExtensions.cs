using System.Linq;
using System.Reflection;
using Hsbot.Core;
using Hsbot.Core.Brain;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Hsbot.Hosting.Web
{
    public static class HsbotServiceExtensions
    {
        public static IServiceCollection AddHsbot(this IServiceCollection services, HsbotConfig config)
        {
            RegisterMessageHandlers(services);

            services.AddSingleton<IHsbotLog, HsbotLog>();
            services.AddSingleton<IHsbotConfig>(svc => config);
            services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
            services.AddSingleton<IBotBrainSerializer<HsbotBrain>, JsonBrainSerializer>();
            services.AddSingleton<IBotBrainStorage<HsbotBrain>, AzureBrainStorage>();
            services.AddSingleton<IHsbotChatConnector, HsbotSlackConnector>();
            services.AddSingleton<Core.Hsbot>();

            return services;
        }

        private static void RegisterMessageHandlers(IServiceCollection services)
        {
            var handlerInterfaceType = typeof(IInboundMessageHandler);
            var messageHandlerTypes = Assembly.GetAssembly(typeof(Core.Hsbot))
              .GetTypes()
              .Where(t => !t.IsAbstract && !t.IsInterface && handlerInterfaceType.IsAssignableFrom(t));

            foreach (var messageHandlerType in messageHandlerTypes)
            {
                services.Add(new ServiceDescriptor(handlerInterfaceType, messageHandlerType, ServiceLifetime.Transient));
            }
        }
    }
}
