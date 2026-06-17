namespace Web.Songs
{
    public sealed class ListSongsRequest
    {
        public string Locale { get; set; } = "en-US";
        public long Seed { get; set; } = 12345;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public double Likes { get; set; } = 0;
    }
}
