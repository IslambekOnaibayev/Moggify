using Microsoft.Net.Http.Headers;
using UseCases.Songs.Queries.GetSongAudio;

namespace Web.Songs
{
    public sealed class GetAudio : EndpointWithoutRequest
    {
        private readonly IMediator _mediator;

        public GetAudio(IMediator mediator) => _mediator = mediator;

        public override void Configure()
        {
            Get("/songs/{Seed}/{Index}/audio");
            AllowAnonymous();
            Description(b => b.WithTags("Songs"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var seed = Route<long>("Seed");
            var index = Route<int>("Index");

            var query = new GetSongAudioQuery(seed, index);
            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            HttpContext.Response.Headers.CacheControl = "public, max-age=86400";

            var file = TypedResults.File(
                result.Value,
                contentType: "audio/ogg",
                enableRangeProcessing: true,
                entityTag: new EntityTagHeaderValue($"\"{seed}-{index}\""));

            await file.ExecuteAsync(HttpContext);
        }
    }
}
