using UseCases.Songs.Queries.GetSongCover;

namespace Web.Songs
{
    public sealed class GetCover : EndpointWithoutRequest
    {
        private readonly IMediator _mediator;

        public GetCover(IMediator mediator) => _mediator = mediator;

        public override void Configure()
        {
            Get("/songs/{Seed}/{Index}/cover");
            AllowAnonymous();
            Description(b => b.WithTags("Songs"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var seed = Route<long>("Seed");
            var index = Route<int>("Index");
            var likes = Math.Clamp(Query<double?>("likes") ?? 0, 0, 10);
            var locale = Query<string>("locale") ?? "en-US";
            var title = Query<string>("title") ?? "Untitled";
            var artist = Query<string>("artist") ?? "Unknown";

            var query = new GetSongCoverQuery(seed, index, likes, locale, title, artist);
            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            HttpContext.Response.Headers.CacheControl = "public, max-age=86400";
            HttpContext.Response.Headers.ETag = $"\"{seed}-{index}-{likes}-{locale}\"";
            HttpContext.Response.ContentType = "image/webp";
            await HttpContext.Response.Body.WriteAsync(result.Value, ct);
        }
    }
}
