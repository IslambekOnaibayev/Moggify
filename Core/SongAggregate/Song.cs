namespace Core.SongAggregate
{
    public sealed class Song : EntityBase<Song, SongId>, IAggregateRoot
    {
        public int SequenceIndex { get; private set; }
        public SongTitle Title { get; private set; }
        public ArtistName Artist { get; private set; }
        public AlbumTitle Album { get; private set; }
        public Genre Genre { get; private set; }
        public int Likes { get; private set; }
        public string Label { get; private set; }
        public int Year { get; private set; }
        public string ReviewText { get; private set; }
        public string Lyrics { get; private set; }

        public Song(
            SongId id,
            int sequenceIndex,
            SongTitle title,
            ArtistName artist,
            AlbumTitle album,
            Genre genre,
            int likes,
            string label,
            int year,
            string reviewText,
            string lyrics)
        {
            Guard.Against.NegativeOrZero(sequenceIndex, nameof(sequenceIndex));
            Guard.Against.Negative(likes, nameof(likes));

            Id = id;
            SequenceIndex = sequenceIndex;
            Title = title;
            Artist = artist;
            Album = album;
            Genre = genre;
            Likes = likes;
            Label = label;
            Year = year;
            ReviewText = reviewText;
            Lyrics = lyrics;
        }
    }
}
