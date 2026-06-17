namespace Infrastructure.AudioGeneration
{
    internal static class WavEncoder
    {
        private const short Channels = 1;
        private const short BitsPerSample = 16;

        public static byte[] Encode(float[] monoSamples, int sampleRate)
        {
            var sampleCount = monoSamples.Length;
            var dataSize = sampleCount * Channels * (BitsPerSample / 8);

            using var ms = new System.IO.MemoryStream();
            using var writer = new System.IO.BinaryWriter(ms);

            writer.Write(new[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + dataSize);
            writer.Write(new[] { 'W', 'A', 'V', 'E' });
            writer.Write(new[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write(Channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * Channels * (BitsPerSample / 8));
            writer.Write((short)(Channels * (BitsPerSample / 8)));
            writer.Write(BitsPerSample);
            writer.Write(new[] { 'd', 'a', 't', 'a' });
            writer.Write(dataSize);

            foreach (var sample in monoSamples)
            {
                var clamped = Math.Clamp(sample, -1f, 1f);
                var pcm = (short)(clamped * short.MaxValue);
                writer.Write(pcm);
            }

            return ms.ToArray();
        }
    }
}
