using API.Trade.Shared.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace API.Trade.Shared.Messaging
{
    public static class Startup
    {
        public static void AddMessagingDependencies(this IServiceCollection services)
        {
            services.AddScoped<ITradeMessaging, TradeMessaging>();
        }

        public static void AddDefaultMessagingBehaviours(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }

        public static void AddMessagingObjects(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);
        }
    }
}
