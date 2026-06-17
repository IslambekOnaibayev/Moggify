using UseCases.Songs.Queries.ListSongs;

namespace Web.Configurations
{
    public static class MediatorConfig
    {
        public static IServiceCollection AddMediatorSourceGen(this IServiceCollection services,
            Microsoft.Extensions.Logging.ILogger logger)
        {
            logger.LogInformation("Registering Mediator SourceGen and Behaviors");
            services.AddMediator(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.Assemblies =
                [
                    typeof(Song),
                    typeof(ListSongsQuery),
                    typeof(InfrastructureServiceExtensions),
                    typeof(MediatorConfig)
                ];
                options.PipelineBehaviors =
                [
                    typeof(LoggingBehavior<,>)
                ];
            });

            return services;
        }
    }
}
