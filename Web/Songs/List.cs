using UseCases.Songs.Queries.ListSongs;

namespace Web.Songs
{
    public sealed class List : Endpoint<ListSongsRequest, PagedResult<SongDto>>
    {
        private readonly IMediator _mediator;

        public List(IMediator mediator) => _mediator = mediator;

        public override void Configure()
        {
            Get("/songs");
            AllowAnonymous();
            Description(b => b.WithTags("Songs"));
        }

        public override async Task HandleAsync(ListSongsRequest req, CancellationToken ct)
        {
            var pageSize = Math.Clamp(req.PageSize, 1, 100);
            var page = Math.Max(1, req.Page);

            var query = new ListSongsQuery(
                req.Locale,
                req.Seed,
                page,
                pageSize,
                Math.Clamp(req.Likes, 0, 10));

            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
            {
                HttpContext.Response.StatusCode = 500;
                return;
            }

            await HttpContext.Response.WriteAsJsonAsync(result.Value, ct);
        }
    }
}
