namespace Web.Songs
{
    public sealed class GetSongDetailsRequest
    {
        public int Index { get; set; }
        public string Locale { get; set; } = "en-US";
        public long Seed { get; set; } = 12345;
        public double Likes { get; set; } = 0;
    }
}
