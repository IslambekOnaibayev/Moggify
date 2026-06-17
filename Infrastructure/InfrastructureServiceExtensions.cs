using Infrastructure.AudioGeneration;
using Infrastructure.CoverGeneration;
using Infrastructure.SongGeneration;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            string environmentName)
        {
            services.AddSingleton<ISongGenerationService, BogusSongGenerationService>();
            services.AddSingleton<ISongAudioService, DryWetMidiAudioService>();
            services.AddSingleton<ISongCoverService, SkiaSharpCoverService>();

            logger.LogInformation("{Project} services registered", "Infrastructure");

            return services;
        }
    }
}
