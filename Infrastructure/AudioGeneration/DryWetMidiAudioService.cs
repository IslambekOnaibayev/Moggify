using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.AudioGeneration
{
    internal sealed class DryWetMidiAudioService : ISongAudioService
    {
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromHours(6);

        private readonly IMemoryCache _cache;

        public DryWetMidiAudioService(IMemoryCache cache) => _cache = cache;

        public byte[] GenerateAudio(long userSeed, int globalIndex)
        {
            var key = (userSeed, globalIndex);

            var lazy = _cache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = CacheLifetime;
                return new Lazy<byte[]>(() => Render(userSeed, globalIndex));
            })!;

            return lazy.Value;
        }

        private static byte[] Render(long userSeed, int globalIndex)
        {
            var song = MusicComposer.Compose(userSeed, globalIndex);
            var samples = AudioRenderer.Render(song);
            return OggOpusEncoder.Encode(samples, song.SampleRate);
        }
    }
}
