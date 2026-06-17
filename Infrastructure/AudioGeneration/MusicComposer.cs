namespace Infrastructure.AudioGeneration
{
    internal sealed class ComposedSong
    {
        public int TempoMicrosPerBeat { get; init; }
        public int SampleRate { get; init; } = 22050;
        public List<NoteEvent> Notes { get; init; } = [];
    }

    internal sealed class NoteEvent
    {
        public double StartSeconds { get; init; }
        public double DurationSeconds { get; init; }
        public int MidiNote { get; init; }
        public float Velocity { get; init; }
        public InstrumentType Instrument { get; init; }
    }

    internal enum InstrumentType { Drum, Bass, Pad, Lead, Pluck, Organ }

    internal sealed record StyleProfile(
        int BpmMin,
        int BpmMax,
        double MinorProb,
        InstrumentType ChordInstrument,
        InstrumentType MelodyInstrument,
        bool ArpeggiateChords,
        bool WalkingBass,
        double DrumIntensity);

    internal static class MusicComposer
    {
        private static readonly int[] MajorScale = [0, 2, 4, 5, 7, 9, 11];
        private static readonly int[] MinorScale = [0, 2, 3, 5, 7, 8, 10];

        private static readonly int[][] ChordProgressions =
        [
            [0, 3, 4, 0],
            [0, 4, 5, 4],
            [0, 5, 3, 4],
            [0, 3, 0, 4],
            [5, 3, 0, 4],
            [0, 2, 5, 4],
            [0, 4, 1, 5],
            [3, 4, 0, 5],
        ];

        private static readonly int[] MajorChordIntervals = [0, 4, 7];
        private static readonly int[] MinorChordIntervals = [0, 3, 7];

        private static readonly int[][] RhythmPatterns =
        [
            [2, 2, 2, 2],
            [2, 2, 4],
            [4, 2, 2],
            [2, 1, 1, 4],
            [1, 1, 2, 2, 2],
            [2, 2, 2, 1, 1],
            [3, 1, 2, 2],
            [4, 4],
        ];

        private static readonly StyleProfile[] Styles =
        [
            new(100, 126, 0.4, InstrumentType.Pad,   InstrumentType.Lead,  false, false, 0.9),
            new(112, 144, 0.5, InstrumentType.Organ, InstrumentType.Lead,  false, false, 1.0),
            new(68,  90,  0.7, InstrumentType.Pad,   InstrumentType.Pluck, false, false, 0.4),
            new(120, 132, 0.5, InstrumentType.Pad,   InstrumentType.Lead,  true,  false, 1.0),
            new(88,  118, 0.35, InstrumentType.Pluck, InstrumentType.Lead, true,  false, 0.5),
            new(84,  118, 0.6, InstrumentType.Organ, InstrumentType.Lead,  false, true,  0.6),
        ];

        public static ComposedSong Compose(long userSeed, int globalIndex)
        {
            var seed = (int)((userSeed * globalIndex + globalIndex) & 0x7FFFFFFF);
            var rng = new Random(seed);

            var style = Styles[rng.Next(Styles.Length)];
            var bpm = rng.Next(style.BpmMin, style.BpmMax + 1);
            var secondsPerBeat = 60.0 / bpm;
            var root = rng.Next(48, 60);
            var isMinor = rng.NextDouble() < style.MinorProb;
            var scale = isMinor ? MinorScale : MajorScale;
            var chordProg = ChordProgressions[rng.Next(ChordProgressions.Length)];
            const int beatsPerBar = 4;
            var secondsPerBar = beatsPerBar * secondsPerBeat;

            var targetSeconds = 120 + rng.Next(0, 121);
            var bars = Math.Max(16, (int)Math.Round(targetSeconds / secondsPerBar));

            var song = new ComposedSong
            {
                TempoMicrosPerBeat = (int)(60_000_000.0 / bpm),
                Notes = []
            };

            for (var bar = 0; bar < bars; bar++)
            {
                var chordDegree = chordProg[bar % chordProg.Length];
                var chordRoot = root + scale[chordDegree % scale.Length];
                var chordIntervals = isMinor ? MinorChordIntervals : MajorChordIntervals;

                AddChords(song, style, bar, beatsPerBar, secondsPerBeat, chordRoot, chordIntervals, rng);
                AddBassLine(song, style, bar, beatsPerBar, secondsPerBeat, chordRoot, scale, rng);
            }

            AddMelodyLine(song, style, bars, beatsPerBar, secondsPerBeat, scale, root, chordProg, rng);
            AddDrums(song, style, bars, beatsPerBar, secondsPerBeat, rng);

            return song;
        }

        private static void AddChords(ComposedSong song, StyleProfile style, int bar, int beatsPerBar,
            double secondsPerBeat, int chordRoot, int[] intervals, Random rng)
        {
            var barStart = bar * beatsPerBar * secondsPerBeat;

            if (style.ArpeggiateChords)
            {
                var steps = beatsPerBar * 2;
                for (var s = 0; s < steps; s++)
                {
                    var interval = intervals[s % intervals.Length] + (s >= intervals.Length ? 12 : 0);
                    song.Notes.Add(new NoteEvent
                    {
                        StartSeconds = barStart + s * secondsPerBeat * 0.5,
                        DurationSeconds = secondsPerBeat * 0.5 * 0.9,
                        MidiNote = chordRoot + interval,
                        Velocity = 0.32f + (float)rng.NextDouble() * 0.1f,
                        Instrument = style.ChordInstrument
                    });
                }
            }
            else
            {
                foreach (var interval in intervals)
                {
                    song.Notes.Add(new NoteEvent
                    {
                        StartSeconds = barStart,
                        DurationSeconds = beatsPerBar * secondsPerBeat * 0.95,
                        MidiNote = chordRoot + interval,
                        Velocity = 0.3f + (float)rng.NextDouble() * 0.1f,
                        Instrument = style.ChordInstrument
                    });
                }
            }
        }

        private static void AddBassLine(ComposedSong song, StyleProfile style, int bar, int beatsPerBar,
            double secondsPerBeat, int chordRoot, int[] scale, Random rng)
        {
            var bassRoot = chordRoot - 12;

            if (style.WalkingBass)
            {
                for (var beat = 0; beat < beatsPerBar; beat++)
                {
                    var degree = scale[(beat * 2) % scale.Length];
                    song.Notes.Add(new NoteEvent
                    {
                        StartSeconds = (bar * beatsPerBar + beat) * secondsPerBeat,
                        DurationSeconds = secondsPerBeat * 0.9,
                        MidiNote = bassRoot + degree,
                        Velocity = 0.55f + (float)rng.NextDouble() * 0.1f,
                        Instrument = InstrumentType.Bass
                    });
                }
                return;
            }

            foreach (var beat in new[] { 0, 2 })
            {
                song.Notes.Add(new NoteEvent
                {
                    StartSeconds = (bar * beatsPerBar + beat) * secondsPerBeat,
                    DurationSeconds = secondsPerBeat * 0.8,
                    MidiNote = bassRoot,
                    Velocity = 0.6f + (float)rng.NextDouble() * 0.15f,
                    Instrument = InstrumentType.Bass
                });
            }

            if (rng.NextDouble() > 0.4)
            {
                song.Notes.Add(new NoteEvent
                {
                    StartSeconds = (bar * beatsPerBar + 3) * secondsPerBeat,
                    DurationSeconds = secondsPerBeat * 0.5,
                    MidiNote = bassRoot + 5,
                    Velocity = 0.5f + (float)rng.NextDouble() * 0.1f,
                    Instrument = InstrumentType.Bass
                });
            }
        }

        private static void AddMelodyLine(ComposedSong song, StyleProfile style, int bars, int beatsPerBar,
            double secondsPerBeat, int[] scale, int root, int[] chordProg, Random rng)
        {
            var melodyBase = root + 12;
            var rhythm = RhythmPatterns[rng.Next(RhythmPatterns.Length)];

            var motif = new int[rhythm.Length];
            var degree = 0;
            var eighthPos = 0;
            for (var n = 0; n < rhythm.Length; n++)
            {
                var strong = eighthPos == 0 || eighthPos == beatsPerBar;
                if (strong)
                    degree = new[] { 0, 2, 4 }[rng.Next(3)];
                else
                    degree += rng.NextDouble() switch { < 0.5 => 1, < 0.85 => -1, < 0.93 => 2, _ => -2 };
                motif[n] = degree;
                eighthPos += rhythm[n];
            }

            const int phraseLen = 4;
            for (var bar = 1; bar < bars - 1; bar++)
            {
                if (bar % 8 == 7) continue;

                var chordDegree = chordProg[bar % chordProg.Length];
                var octaveShift = (bar / phraseLen) % 2 == 1 ? scale.Length : 0;
                var isCadence = bar % phraseLen == phraseLen - 1;

                var eighth = 0;
                for (var n = 0; n < rhythm.Length; n++)
                {
                    var isLast = n == rhythm.Length - 1;
                    var deg = chordDegree + motif[n] + octaveShift;
                    if (isCadence && isLast) deg = chordDegree;
                    deg = Math.Clamp(deg, 0, 14);

                    var strong = eighth == 0 || eighth == beatsPerBar;
                    song.Notes.Add(new NoteEvent
                    {
                        StartSeconds = (bar * beatsPerBar + eighth * 0.5) * secondsPerBeat,
                        DurationSeconds = secondsPerBeat * 0.5 * rhythm[n] * 0.92,
                        MidiNote = DegreeToPitch(melodyBase, scale, deg),
                        Velocity = (strong ? 0.6f : 0.48f) + (float)rng.NextDouble() * 0.08f,
                        Instrument = style.MelodyInstrument
                    });
                    eighth += rhythm[n];
                }
            }
        }

        private static int DegreeToPitch(int basePitch, int[] scale, int degree)
        {
            var idx = degree % scale.Length;
            var oct = degree / scale.Length;
            if (idx < 0) { idx += scale.Length; oct--; }
            return basePitch + oct * 12 + scale[idx];
        }

        private static void AddDrums(ComposedSong song, StyleProfile style, int bars, int beatsPerBar,
            double secondsPerBeat, Random rng)
        {
            const int kick = 36, snare = 38, hihat = 42;

            for (var bar = 0; bar < bars; bar++)
            {
                for (var beat = 0; beat < beatsPerBar; beat++)
                {
                    var t = (bar * beatsPerBar + beat) * secondsPerBeat;

                    if (beat == 0 || beat == 2)
                        Add(song, t, secondsPerBeat * 0.2, kick, 0.85f);

                    if (beat == 1 || beat == 3)
                        Add(song, t, secondsPerBeat * 0.15, snare, 0.7f);

                    var subdivisions = style.DrumIntensity >= 0.9 ? 4 : style.DrumIntensity >= 0.5 ? 2 : 1;
                    for (var s = 0; s < subdivisions; s++)
                    {
                        Add(song, t + s * secondsPerBeat / subdivisions, secondsPerBeat * 0.1, hihat,
                            0.14f + (float)rng.NextDouble() * 0.12f);
                    }
                }
            }

            static void Add(ComposedSong song, double start, double dur, int note, float vel) =>
                song.Notes.Add(new NoteEvent
                {
                    StartSeconds = start,
                    DurationSeconds = dur,
                    MidiNote = note,
                    Velocity = vel,
                    Instrument = InstrumentType.Drum
                });
        }
    }
}
