namespace Infrastructure.SongGeneration
{
    internal sealed class LocaleSongData
    {
        public string[] Genres { get; set; } = [];
        public string[] TitleAdjectives { get; set; } = [];
        public string[] TitleNouns { get; set; } = [];
        public string[] TitleTemplates { get; set; } = [];
        public string[] AlbumAdjectives { get; set; } = [];
        public string[] AlbumNouns { get; set; } = [];
        public string[] BandPrefixes { get; set; } = [];
        public string[] BandNouns { get; set; } = [];
        public string[] FirstNames { get; set; } = [];
        public string[] LastNames { get; set; } = [];
        public string[] Suffixes { get; set; } = [];
        public string[] Labels { get; set; } = [];
        public string[] ReviewLines { get; set; } = [];
        public string[] LyricsLines { get; set; } = [];
    }
}
