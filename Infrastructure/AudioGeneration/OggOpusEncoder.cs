using Concentus;
using Concentus.Enums;
using Concentus.Oggfile;

namespace Infrastructure.AudioGeneration
{
    internal static class OggOpusEncoder
    {
        private const int Bitrate = 96000;

        public static byte[] Encode(float[] samples, int sampleRate)
        {
            var encoder = OpusCodecFactory.CreateEncoder(sampleRate, 1, OpusApplication.OPUS_APPLICATION_AUDIO);
            encoder.Bitrate = Bitrate;

            var shorts = new short[samples.Length];
            for (var i = 0; i < samples.Length; i++)
            {
                var clamped = Math.Clamp(samples[i], -1f, 1f);
                shorts[i] = (short)(clamped * short.MaxValue);
            }

            using var ms = new MemoryStream();
            var writer = new OpusOggWriteStream(encoder, ms, null, sampleRate, 1);
            writer.WriteSamples(shorts, 0, shorts.Length);
            writer.Finish();

            return ms.ToArray();
        }
    }
}
