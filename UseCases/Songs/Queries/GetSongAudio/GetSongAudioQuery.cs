namespace UseCases.Songs.Queries.GetSongAudio
{
    public record GetSongAudioQuery(
        long UserSeed,
        int GlobalIndex) : IQuery<Result<byte[]>>;
}
