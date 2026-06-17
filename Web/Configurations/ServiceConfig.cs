namespace Web.Configurations
{
    public static class ServiceConfig
    {
        public static IServiceCollection AddServiceConfigs(this IServiceCollection services,
                                                           Microsoft.Extensions.Logging.ILogger logger,
                                                           WebApplicationBuilder builder)
        {
            services.AddCoreServices(logger)
                    .AddInfrastructureServices(builder.Configuration, logger, builder.Environment.EnvironmentName)
                    .AddMediatorSourceGen(logger);

            return services;
        }
    }
}
