using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.AudioGeneration
{
    internal sealed class DryWetMidiAudioService : ISongAudioService
    {
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromHours(6);

        private readonly IMemoryCache _cache;

        public DryWetMidiAudioService(IMemoryCache cache) => _cache = cache;

        // Rendering + Opus encoding a 40-60s track costs several CPU-seconds. On a
        // throttled host the response can exceed the gateway timeout and get cut off,
        // so the player receives a truncated clip. Each track is deterministic for a
        // given (seed, index), so we render it once and serve the cached bytes for
        // every later request (including range/seek requests from the audio element).
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
