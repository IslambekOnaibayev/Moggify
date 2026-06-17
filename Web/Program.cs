using Web.Configurations;

namespace Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var invariant = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = invariant;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = invariant;

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureKestrel(o =>
            {
                o.AddServerHeader = false;
            });

            var port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrWhiteSpace(port))
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }

            var logger = Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            logger.Information("Starting web host");

            builder.AddLoggerConfigs();

            var appLogger = new SerilogLoggerFactory(logger).CreateLogger<Program>();

            builder.Services.AddOptionConfigs(builder.Configuration, appLogger, builder);
            builder.Services.AddServiceConfigs(appLogger, builder);

            builder.Services.AddFastEndpoints()
                            .SwaggerDocument(o => { o.ShortSchemaNames = true; });

            builder.Services.ConfigureHttpJsonOptions(o =>
            {
                o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddMemoryCache();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(
                              builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? ["http://localhost:4200"])
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            await app.UseAppMiddleware();

            app.Run();
        }
    }
}

public partial class Program { }
