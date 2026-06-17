namespace Core.SongAggregate
{
    [ValueObject<string>]
    public partial class ArtistName
    {
        private static Validation Validate(string value)
            => !string.IsNullOrWhiteSpace(value)
                ? Validation.Ok
                : Validation.Invalid("Artist name cannot be empty.");
    }
}
