using Core.Interfaces;

namespace UseCases.Songs.Queries.ListSongs
{
    public sealed class ListSongsHandler(ISongGenerationService songGenerator)
        : IQueryHandler<ListSongsQuery, Result<PagedResult<SongDto>>>
    {
        public ValueTask<Result<PagedResult<SongDto>>> Handle(
            ListSongsQuery query,
            CancellationToken cancellationToken)
        {
            var songs = songGenerator.GeneratePage(
                query.Locale,
                query.UserSeed,
                query.Page,
                query.PageSize,
                query.LikesAvg);

            var dtos = songs.Select(s => new SongDto(
                s.SequenceIndex,
                s.Title.Value,
                s.Artist.Value,
                s.Album.Value,
                s.Genre.Value,
                s.Likes)).ToList();

            var result = new PagedResult<SongDto>(
                dtos,
                query.Page,
                query.PageSize,
                int.MaxValue,
                int.MaxValue);

            return ValueTask.FromResult(Result<PagedResult<SongDto>>.Success(result));
        }
    }
}
