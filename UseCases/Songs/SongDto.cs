namespace UseCases.Songs
{
    public record SongDto(
        int Index,
        string Title,
        string Artist,
        string Album,
        string Genre,
        int Likes);
}
