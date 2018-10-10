using System.Linq;
using System.Reflection;
using Hsbot.Slack.Core;
using Hsbot.Slack.Core.Messaging;
using Hsbot.Slack.Core.Random;
using Microsoft.Extensions.DependencyInjection;

namespace Hsbot.Slack.Web
{
    public static class HsbotServiceExtensions
    {
        public static IServiceCollection AddHsbot(this IServiceCollection services, HsbotConfig config)
        {
            RegisterMessageHandlers(services);

            services.AddSingleton<IHsbotLog, HsbotLog>();
            services.AddSingleton<IHsbotConfig>(svc => config);
            services.AddSingleton<IRandomNumberGenerator, RandomNumberGenerator>();
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
