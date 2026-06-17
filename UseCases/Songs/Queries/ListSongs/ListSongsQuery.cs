namespace UseCases.Songs.Queries.ListSongs
{
    public record ListSongsQuery(
        string Locale,
        long UserSeed,
        int Page,
        int PageSize,
        double LikesAvg) : IQuery<Result<PagedResult<SongDto>>>;
}
