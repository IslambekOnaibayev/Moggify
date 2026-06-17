namespace Core.SongAggregate
{
    [ValueObject<string>]
    public partial class Genre
    {
        private static Validation Validate(string value)
            => !string.IsNullOrWhiteSpace(value)
                ? Validation.Ok
                : Validation.Invalid("Genre cannot be empty.");
    }
}
