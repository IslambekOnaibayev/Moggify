namespace UseCases.Songs.Queries.GetSongCover
{
    public record GetSongCoverQuery(
        long UserSeed,
        int GlobalIndex,
        double LikesAvg,
        string Locale,
        string Title,
        string Artist) : IQuery<Result<byte[]>>;
}
