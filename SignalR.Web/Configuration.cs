using AutoMapper;
using SolarLab.BusManager.Abstraction;
using SolarLab.BusManager.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SignalR.Web
{
    public static class Configuration
    {
        public static IServiceCollection InstallConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddSingleton(configuration)
                .AddSingleton((IConfigurationRoot)configuration);
        }

        public static IServiceCollection ConfigureBusManager(this IServiceCollection services) => services
            .AddSingleton<IBusManager, MassTransitBusManager>();

        public static IServiceCollection ConfigureAutomapper(this IServiceCollection services) => services
            .AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));

        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                //cfg.AddProfile<MapProfile>();
            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }
    }
}
