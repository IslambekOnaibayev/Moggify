using Core.Interfaces;

namespace UseCases.Songs.Queries.GetSongDetails
{
    public sealed class GetSongDetailsHandler(ISongGenerationService songGenerator)
        : IQueryHandler<GetSongDetailsQuery, Result<SongDetailsDto>>
    {
        public ValueTask<Result<SongDetailsDto>> Handle(
            GetSongDetailsQuery query,
            CancellationToken cancellationToken)
        {
            var song = songGenerator.GenerateOne(
                query.Locale,
                query.UserSeed,
                query.GlobalIndex,
                query.LikesAvg);

            var dto = new SongDetailsDto(
                song.SequenceIndex,
                song.Title.Value,
                song.Artist.Value,
                song.Album.Value,
                song.Genre.Value,
                song.Likes,
                song.Label,
                song.Year,
                song.ReviewText,
                song.Lyrics);

            return ValueTask.FromResult(Result<SongDetailsDto>.Success(dto));
        }
    }
}
