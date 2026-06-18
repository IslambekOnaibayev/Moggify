using Bogus;

namespace Infrastructure.SongGeneration
{
    internal sealed class BogusSongGenerationService : ISongGenerationService
    {
        private const int PageSize = 10;

        public IReadOnlyList<Song> GeneratePage(string locale, long userSeed, int page, int pageSize, double likesAvg)
        {
            var songs = new List<Song>(pageSize);
            for (var i = 0; i < pageSize; i++)
            {
                var globalIndex = (page - 1) * pageSize + i + 1;
                songs.Add(GenerateOne(locale, userSeed, globalIndex, likesAvg));
            }
            return songs;
        }

        public Song GenerateOne(string locale, long userSeed, int globalIndex, double likesAvg)
        {
            var songSeed = ComputeSongSeed(userSeed, globalIndex, likesAvg);
            var data = SongDataProvider.GetData(locale);
            var bogusLocale = MapToBogusLocale(locale);
            var faker = new Faker(bogusLocale) { Random = new Randomizer((int)(songSeed & 0x7FFFFFFF)) };

            var title = GenerateTitle(faker, data);
            var artist = GenerateArtist(faker, data);
            var album = GenerateAlbum(faker, data);
            var genre = Pick(faker, data.Genres);
            var likes = ComputeLikes(songSeed, likesAvg);
            var label = Pick(faker, data.Labels);
            var year = faker.Random.Int(1970, 2025);
            var review = GenerateReview(faker, data);
            var lyrics = GenerateLyrics(faker, data);

            return new Song(
                SongId.From(globalIndex),
                globalIndex,
                SongTitle.From(title),
                ArtistName.From(artist),
                AlbumTitle.From(album),
                Genre.From(genre),
                likes,
                label,
                year,
                review,
                lyrics);
        }

        private static long ComputeSongSeed(long userSeed, int globalIndex, double likesAvg)
        {
            var likesComponent = (long)Math.Round(likesAvg * 10);
            var seed = userSeed + likesComponent * 2_654_435_761L;
            return seed * globalIndex + globalIndex;
        }

        private static int ComputeLikes(long songSeed, double likesAvg)
        {
            if (likesAvg <= 0) return 0;
            var baseLikes = (int)Math.Floor(likesAvg);
            var fractional = likesAvg - baseLikes;
            var rng = new Random((int)((songSeed ^ 0xDEAD_BEEF) & 0x7FFFFFFF));
            return baseLikes + (rng.NextDouble() < fractional ? 1 : 0);
        }

        private static string GenerateTitle(Faker faker, LocaleSongData data)
        {
            var template = Pick(faker, data.TitleTemplates);
            var adj = Pick(faker, data.TitleAdjectives);
            var adj2 = PickDistinct(faker, data.TitleAdjectives, adj);
            var noun = Pick(faker, data.TitleNouns);
            var noun2 = PickDistinct(faker, data.TitleNouns, noun);

            return template
                .Replace("{adj2}", adj2)
                .Replace("{noun2}", noun2)
                .Replace("{adj}", adj)
                .Replace("{noun}", noun);
        }

        private static string GenerateArtist(Faker faker, LocaleSongData data)
        {
            if (faker.Random.Bool(0.4f))
            {
                var prefix = faker.Random.Bool(0.5f) ? Pick(faker, data.BandPrefixes) + " " : "";
                return prefix + Pick(faker, data.BandNouns);
            }

            var firstName = Pick(faker, data.FirstNames);
            var lastName = Pick(faker, data.LastNames);
            var full = $"{firstName} {lastName}";

            if (data.Suffixes.Length > 0 && faker.Random.Bool(0.15f))
                full += $" {Pick(faker, data.Suffixes)}";

            return full;
        }

        private static string GenerateAlbum(Faker faker, LocaleSongData data)
        {
            if (faker.Random.Bool(0.35f)) return "Single";
            return $"{Pick(faker, data.AlbumAdjectives)} {Pick(faker, data.AlbumNouns)}";
        }

        private static string GenerateReview(Faker faker, LocaleSongData data)
        {
            var lineCount = faker.Random.Int(2, 4);
            var lines = faker.Random.Shuffle(data.ReviewLines).Take(lineCount);
            return string.Join(" ", lines);
        }

        private static string GenerateLyrics(Faker faker, LocaleSongData data)
        {
            var allLines = faker.Random.Shuffle(data.LyricsLines).ToArray();
            if (allLines.Length == 0) return string.Empty;

            var sb = new System.Text.StringBuilder();

            void AddVerse(int start, int count)
            {
                for (var i = start; i < start + count; i++)
                    sb.AppendLine(allLines[i % allLines.Length]);
                sb.AppendLine();
            }

            AddVerse(0, 4);
            AddVerse(4, 4);
            AddVerse(8, 5);

            return sb.ToString().Trim();
        }

        private static string Pick(Faker faker, string[] arr)
            => arr.Length > 0 ? arr[faker.Random.Int(0, arr.Length - 1)] : string.Empty;

        private static string PickDistinct(Faker faker, string[] arr, string exclude)
        {
            if (arr.Length <= 1) return Pick(faker, arr);
            string value;
            var guard = 0;
            do { value = Pick(faker, arr); } while (value == exclude && guard++ < 8);
            return value;
        }

        private static string MapToBogusLocale(string locale) => locale switch
        {
            "ru-RU" => "ru",
            "kk-KZ" => "ru",
            "be-BY" => "ru",
            _ => "en"
        };
    }
}
