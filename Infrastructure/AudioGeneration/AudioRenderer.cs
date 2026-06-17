namespace Infrastructure.AudioGeneration
{
    internal static class AudioRenderer
    {
        private const float TwoPi = 2f * MathF.PI;

        public static float[] Render(ComposedSong song)
        {
            var sampleRate = song.SampleRate;
            var lastNote = song.Notes.Count > 0
                ? song.Notes.Max(n => n.StartSeconds + n.DurationSeconds)
                : 4.0;
            var totalSamples = (int)((lastNote + 1.5) * sampleRate);
            var buffer = new float[totalSamples];

            foreach (var note in song.Notes)
            {
                var samples = RenderNote(note, sampleRate);
                var startSample = (int)(note.StartSeconds * sampleRate);

                for (var i = 0; i < samples.Length && (startSample + i) < buffer.Length; i++)
                    buffer[startSample + i] += samples[i];
            }

            ApplyReverb(buffer, sampleRate);
            Normalize(buffer);

            return buffer;
        }

        private static float[] RenderNote(NoteEvent note, int sampleRate)
        {
            var sampleCount = (int)(note.DurationSeconds * sampleRate) + (int)(sampleRate * 0.3);
            var samples = new float[sampleCount];
            var freq = MidiToHz(note.MidiNote);

            switch (note.Instrument)
            {
                case InstrumentType.Drum:
                    RenderDrum(samples, freq, note.Velocity, sampleRate, note.MidiNote);
                    break;
                case InstrumentType.Bass:
                    RenderBass(samples, freq, note.Velocity, sampleRate, note.DurationSeconds);
                    break;
                case InstrumentType.Pad:
                    RenderPad(samples, freq, note.Velocity, sampleRate, note.DurationSeconds);
                    break;
                case InstrumentType.Lead:
                    RenderLead(samples, freq, note.Velocity, sampleRate, note.DurationSeconds);
                    break;
                case InstrumentType.Pluck:
                    RenderPluck(samples, freq, note.Velocity, sampleRate);
                    break;
                case InstrumentType.Organ:
                    RenderOrgan(samples, freq, note.Velocity, sampleRate, note.DurationSeconds);
                    break;
            }

            ApplyEdgeFade(samples, sampleRate);
            return samples;
        }

        private static void ApplyEdgeFade(float[] samples, int sampleRate)
        {
            var fade = Math.Min(samples.Length / 2, (int)(sampleRate * 0.004));
            if (fade <= 0) return;
            for (var i = 0; i < fade; i++)
            {
                var g = (float)i / fade;
                samples[i] *= g;
                samples[samples.Length - 1 - i] *= g;
            }
        }

        private static void RenderDrum(float[] samples, float freq, float velocity, int sampleRate, int midiNote)
        {
            var attackSamples = (int)(0.004 * sampleRate);
            var isKick = midiNote == 36;
            var isSnare = midiNote == 38;
            var isHihat = midiNote == 42;

            var rng = new Random(midiNote * 1000);

            for (var i = 0; i < samples.Length; i++)
            {
                var t = (float)i / sampleRate;

                float envelope;
                if (i < attackSamples)
                    envelope = (float)i / attackSamples;
                else
                    envelope = MathF.Exp(-(t - attackSamples / (float)sampleRate) / 0.08f);

                float sample;
                if (isKick)
                {
                    var kickFreq = freq * MathF.Exp(-t * 5f);
                    sample = MathF.Sin(TwoPi * kickFreq * t) * envelope;
                }
                else if (isSnare)
                {
                    var noise = (float)(rng.NextDouble() * 2 - 1);
                    sample = (noise * 0.6f + MathF.Sin(TwoPi * freq * t) * 0.3f) * envelope;
                }
                else if (isHihat)
                {
                    var noise = (float)(rng.NextDouble() * 2 - 1);
                    sample = noise * MathF.Exp(-t * 45f);
                }
                else
                {
                    sample = MathF.Sin(TwoPi * freq * t) * envelope;
                }

                samples[i] = sample * velocity * 0.55f;
            }
        }

        private static void RenderBass(float[] samples, float freq, float velocity, int sampleRate, double duration)
        {
            var noteSamples = (int)(duration * sampleRate);
            var dt = freq / sampleRate;
            var phase = 0f;
            for (var i = 0; i < samples.Length; i++)
            {
                var t = (float)i / sampleRate;
                var envelope = i < noteSamples
                    ? MathF.Min(1f, i / (sampleRate * 0.01f)) * MathF.Exp(-t * 2f)
                    : MathF.Exp(-(t - (float)duration) * 8f);

                var saw = 2f * phase - 1f - PolyBlep(phase, dt);
                samples[i] = saw * envelope * velocity * 0.5f;

                phase += dt;
                if (phase >= 1f) phase -= 1f;
            }
        }

        private static void RenderPad(float[] samples, float freq, float velocity, int sampleRate, double duration)
        {
            var noteSamples = (int)(duration * sampleRate);
            var attackSamples = (int)(0.15 * sampleRate);
            var releaseSamples = (int)(0.3 * sampleRate);

            for (var i = 0; i < samples.Length; i++)
            {
                float envelope;
                if (i < attackSamples)
                    envelope = (float)i / attackSamples;
                else if (i < noteSamples)
                    envelope = 1f;
                else
                    envelope = MathF.Max(0f, 1f - (float)(i - noteSamples) / releaseSamples);

                var phase = TwoPi * freq * i / sampleRate;
                var chorus = MathF.Sin(phase) + MathF.Sin(phase * 1.005f) * 0.5f + MathF.Sin(phase * 0.995f) * 0.5f;
                samples[i] = chorus * envelope * velocity * 0.25f;
            }
        }

        private static void RenderLead(float[] samples, float freq, float velocity, int sampleRate, double duration)
        {
            var noteSamples = (int)(duration * sampleRate);
            var attackSamples = (int)(0.02 * sampleRate);
            var releaseSamples = (int)(0.1 * sampleRate);

            for (var i = 0; i < samples.Length; i++)
            {
                float envelope;
                if (i < attackSamples)
                    envelope = (float)i / attackSamples;
                else if (i < noteSamples)
                    envelope = 0.9f;
                else
                    envelope = MathF.Max(0f, 0.9f - (float)(i - noteSamples) / releaseSamples);

                var phase = (TwoPi * freq * i / sampleRate) % TwoPi;
                var triangle = phase < MathF.PI
                    ? -1f + 2f * phase / MathF.PI
                    : 3f - 2f * phase / MathF.PI;

                samples[i] = triangle * envelope * velocity * 0.4f;
            }
        }

        private static void RenderPluck(float[] samples, float freq, float velocity, int sampleRate)
        {
            var dt = freq / sampleRate;
            var phase = 0f;
            for (var i = 0; i < samples.Length; i++)
            {
                var t = (float)i / sampleRate;
                var envelope = MathF.Exp(-t * 6f);

                var saw = 2f * phase - 1f - PolyBlep(phase, dt);
                samples[i] = saw * envelope * velocity * 0.4f;

                phase += dt;
                if (phase >= 1f) phase -= 1f;
            }
        }

        private static void RenderOrgan(float[] samples, float freq, float velocity, int sampleRate, double duration)
        {
            var noteSamples = (int)(duration * sampleRate);
            var attackSamples = (int)(0.02 * sampleRate);
            var releaseSamples = (int)(0.12 * sampleRate);

            for (var i = 0; i < samples.Length; i++)
            {
                float envelope;
                if (i < attackSamples)
                    envelope = (float)i / attackSamples;
                else if (i < noteSamples)
                    envelope = 1f;
                else
                    envelope = MathF.Max(0f, 1f - (float)(i - noteSamples) / releaseSamples);

                var p = TwoPi * freq * i / sampleRate;
                var tone = MathF.Sin(p)
                         + 0.5f * MathF.Sin(p * 2f)
                         + 0.3f * MathF.Sin(p * 3f);
                samples[i] = tone * envelope * velocity * 0.18f;
            }
        }

        private static void ApplyReverb(float[] buffer, int sampleRate)
        {
            var delays = new[] { 0.029f, 0.037f, 0.041f, 0.043f };
            var decays = new[] { 0.6f, 0.55f, 0.5f, 0.45f };
            var wet = 0.2f;

            foreach (var (delay, decay) in delays.Zip(decays))
            {
                var delaySamples = (int)(delay * sampleRate);
                for (var i = delaySamples; i < buffer.Length; i++)
                    buffer[i] += buffer[i - delaySamples] * decay * wet;
            }
        }

        private static void Normalize(float[] buffer)
        {
            var peak = buffer.Length > 0 ? buffer.Max(MathF.Abs) : 0f;
            if (peak <= 0.001f) return;

            var scale = 1.1f / peak;
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = MathF.Tanh(buffer[i] * scale) * 0.92f;
        }

        private static float MidiToHz(int midi)
            => 440f * MathF.Pow(2f, (midi - 69) / 12f);

        private static float PolyBlep(float t, float dt)
        {
            if (t < dt)
            {
                t /= dt;
                return t + t - t * t - 1f;
            }
            if (t > 1f - dt)
            {
                t = (t - 1f) / dt;
                return t * t + t + t + 1f;
            }
            return 0f;
        }
    }
}
