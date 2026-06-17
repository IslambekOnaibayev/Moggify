namespace Web.Configurations
{
    public static class MiddlewareConfig
    {
        public static async Task<IApplicationBuilder> UseAppMiddleware(this WebApplication app)
        {
            app.UseExceptionHandler();
            app.UseCors();

            if (app.Environment.IsDevelopment())
            {
                app.UseShowAllServicesMiddleware();
                app.UseHttpsRedirection();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Serializer.Options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            }).UseSwaggerGen(uiConfig: s => s.AdditionalSettings["filter"] = false);

            app.MapFallbackToFile("index.html");

            await Task.CompletedTask;
            return app;
        }
    }
}
