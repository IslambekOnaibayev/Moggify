namespace Core.SongAggregate
{
    [ValueObject<string>]
    public partial class SongTitle
    {
        private static Validation Validate(string value)
            => !string.IsNullOrWhiteSpace(value)
                ? Validation.Ok
                : Validation.Invalid("Song title cannot be empty.");
    }
}
