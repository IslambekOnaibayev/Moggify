namespace UseCases.Songs.Queries.GetSongDetails
{
    public record GetSongDetailsQuery(
        string Locale,
        long UserSeed,
        int GlobalIndex,
        double LikesAvg) : IQuery<Result<SongDetailsDto>>;
}
