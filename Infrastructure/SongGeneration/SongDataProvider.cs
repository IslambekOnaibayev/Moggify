namespace Infrastructure.SongGeneration
{
    internal static class SongDataProvider
    {
        private static readonly Dictionary<string, LocaleSongData> _cache = new();
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        internal static LocaleSongData GetData(string locale)
        {
            if (_cache.TryGetValue(locale, out var cached)) return cached;

            var resourceName = $"Infrastructure.SongData.{locale}.json";
            var assembly = typeof(SongDataProvider).Assembly;

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? assembly.GetManifestResourceStream($"Infrastructure.SongData.en-US.json")!;

            var data = JsonSerializer.Deserialize<LocaleSongData>(stream, _jsonOptions)
                ?? throw new InvalidOperationException($"Failed to load song data for locale '{locale}'.");

            _cache[locale] = data;
            return data;
        }
    }
}
