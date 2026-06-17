using Core.Interfaces;

namespace UseCases.Songs.Queries.GetSongAudio
{
    public sealed class GetSongAudioHandler(ISongAudioService audioService)
        : IQueryHandler<GetSongAudioQuery, Result<byte[]>>
    {
        public ValueTask<Result<byte[]>> Handle(
            GetSongAudioQuery query,
            CancellationToken cancellationToken)
        {
            var audio = audioService.GenerateAudio(query.UserSeed, query.GlobalIndex);
            return ValueTask.FromResult(Result<byte[]>.Success(audio));
        }
    }
}
