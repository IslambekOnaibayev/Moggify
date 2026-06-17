namespace Core.SongAggregate
{
    [ValueObject<string>]
    public partial class AlbumTitle
    {
        private static Validation Validate(string value)
            => !string.IsNullOrWhiteSpace(value)
                ? Validation.Ok
                : Validation.Invalid("Album title cannot be empty.");
    }
}
