namespace Web.Configurations
{
    public static class OptionConfig
    {
        public static IServiceCollection AddOptionConfigs(this IServiceCollection services,
                                                          IConfiguration configuration,
                                                          Microsoft.Extensions.Logging.ILogger logger,
                                                          WebApplicationBuilder builder)
        {
            services.Configure<CachingOptions>(builder.Configuration.GetSection(CachingOptions.SectionName));

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            if (builder.Environment.IsDevelopment())
            {
                services.Configure((Ardalis.ListStartupServices.ServiceConfig config) =>
                {
                    config.Services = new List<ServiceDescriptor>(builder.Services);
                    config.Path = "/listservices";
                });
            }

            logger.LogInformation("{Project} were configured", "Configuration and Options");

            return services;
        }
    }
}
