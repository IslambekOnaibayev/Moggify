namespace UseCases.Songs
{
    public record SongDetailsDto(
        int Index,
        string Title,
        string Artist,
        string Album,
        string Genre,
        int Likes,
        string Label,
        int Year,
        string ReviewText,
        string Lyrics);
}
