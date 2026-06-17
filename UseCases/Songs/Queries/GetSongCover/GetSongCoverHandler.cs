using Core.Interfaces;

namespace UseCases.Songs.Queries.GetSongCover
{
    public sealed class GetSongCoverHandler(ISongCoverService coverService)
        : IQueryHandler<GetSongCoverQuery, Result<byte[]>>
    {
        public ValueTask<Result<byte[]>> Handle(
            GetSongCoverQuery query,
            CancellationToken cancellationToken)
        {
            var cover = coverService.GenerateCover(
                query.UserSeed,
                query.GlobalIndex,
                query.LikesAvg,
                query.Locale,
                query.Title,
                query.Artist);

            return ValueTask.FromResult(Result<byte[]>.Success(cover));
        }
    }
}
