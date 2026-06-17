using UseCases.Songs.Queries.GetSongDetails;

namespace Web.Songs
{
    public sealed class GetDetails : Endpoint<GetSongDetailsRequest, SongDetailsDto>
    {
        private readonly IMediator _mediator;

        public GetDetails(IMediator mediator) => _mediator = mediator;

        public override void Configure()
        {
            Get("/songs/{Index}/details");
            AllowAnonymous();
            Description(b => b.WithTags("Songs"));
        }

        public override async Task HandleAsync(GetSongDetailsRequest req, CancellationToken ct)
        {
            var query = new GetSongDetailsQuery(
                req.Locale,
                req.Seed,
                req.Index,
                Math.Clamp(req.Likes, 0, 10));

            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            await HttpContext.Response.WriteAsJsonAsync(result.Value, ct);
        }
    }
}
