namespace Infrastructure.AudioGeneration
{
    internal sealed class DryWetMidiAudioService : ISongAudioService
    {
        public byte[] GenerateAudio(long userSeed, int globalIndex)
        {
            var song = MusicComposer.Compose(userSeed, globalIndex);
            var samples = AudioRenderer.Render(song);
            return OggOpusEncoder.Encode(samples, song.SampleRate);
        }
    }
}
