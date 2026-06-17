namespace Core.SongAggregate
{
    [ValueObject<int>]
    public partial struct SongId
    {
        private static Validation Validate(int value)
            => value > 0 ? Validation.Ok : Validation.Invalid("SongId must be positive.");
    }
}
