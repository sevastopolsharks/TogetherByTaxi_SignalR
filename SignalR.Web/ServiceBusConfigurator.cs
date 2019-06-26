using System;
using System.Collections.Generic;
using SevSharks.SignalR.Contracts;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SignalR.Web.Hubs;

namespace SignalR.Web
{
    public static class ServiceBusConfigurator
    {
        /// <summary>
        /// Получить полную конфигурацию для шины
        /// </summary>
        public static Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>> GetBusConfigurations(IServiceProvider serviceProvider)
        {
            var busConfig = new Dictionary<string, Action<IRabbitMqReceiveEndpointConfigurator>>
            {
                {
                    Queues.SignalRQueue,
                    e => e.Handler<SignalREvent>(async ctx =>
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var logger = scope.ServiceProvider.GetService<ILogger<SignalREvent>>();
                            logger.LogDebug($"Пришло событие SignalREventQueue {ctx.Message.Message}");
                            var hubContext = scope.ServiceProvider.GetService<IHubContext<UserHub>>();
                            if (hubContext != null)
                            {
                                var user = hubContext.Clients.User(ctx.Message.UserId);
                                await user.SendAsync("userInfo", new SignalRMessageDto
                                {
                                    Message = ctx.Message.Message,
                                    Type = (int)ctx.Message.Type
                                });
                                return;
                            }

                            logger.LogError("IHubContext<UserHub> не найден");
                            throw new Exception("IHubContext<UserHub> не найден");
                        }
                    })
                }
            };
            return busConfig;
        }
    }
}
