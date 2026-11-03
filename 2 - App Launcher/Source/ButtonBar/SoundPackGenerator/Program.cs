using System.Text;

const int sampleRate = 48_000;
var output = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "DVMS Console"));
var profile = args.Length > 1 && args[1].Equals("set2", StringComparison.OrdinalIgnoreCase)
    ? SoundProfile.Set2
    : SoundProfile.Set1;
Directory.CreateDirectory(output);

var sounds = new (string Name, double Seconds, Action<Synth> Build)[]
{
    ("01 Button Tap.wav", .095, s => { s.Tone(920, 680, 0, .075, .34, Wave.Triangle); s.Noise(0, .035, .08, 700, 5200); }),
    ("02 Button Tap Soft.wav", .12, s => { s.Tone(610, 480, 0, .10, .25, Wave.Sine); s.Tone(1220, 960, .006, .07, .08, Wave.Sine); }),
    ("03 Confirm.wav", .36, s => { s.Ping(610, 0, .14, .28); s.Ping(915, .115, .21, .34); }),
    ("04 Cancel.wav", .34, s => { s.Ping(720, 0, .17, .27); s.Ping(430, .13, .18, .34); }),
    ("05 Attention Chirp.wav", .52, s => { s.Tone(740, 1260, 0, .13, .27, Wave.Sine); s.Tone(1260, 880, .15, .12, .24, Wave.Sine); s.Ping(1040, .31, .16, .24); }),
    ("06 Warning.wav", .82, s => { for (var i=0;i<3;i++) { var t=i*.25; s.Tone(390, 520, t, .14, .28, Wave.Square); s.Ping(780, t+.045, .12, .12); } }),
    ("07 Critical Alert.wav", 1.18, s => { for (var i=0;i<4;i++) { var t=i*.28; s.Tone(510, 860, t, .12, .30, Wave.Saw); s.Tone(860, 510, t+.12, .11, .25, Wave.Saw); } }),
    ("08 Processing Loop.wav", 1.6, s => { for (var i=0;i<8;i++) { var t=i*.19; s.Ping(420+i%4*105, t, .13, .16); s.Ping(840+i%3*140, t+.055, .09, .08); } }),
    ("09 Scan Sweep.wav", 1.35, s => { s.Tone(180, 1850, 0, 1.22, .22, Wave.Sine, 5.5); s.Noise(.05, 1.15, .055, 500, 7200); s.Ping(1320, 1.11, .18, .20); }),
    ("10 Data Transfer.wav", 1.15, s => { for (var i=0;i<14;i++) { var t=.035+i*.073; var f=520+((i*7)%9)*115; s.Ping(f,t,.055,.12); } s.Tone(1600, 520,.05,.98,.055,Wave.Sine,13); }),
    ("11 System Startup.wav", 1.72, s => { s.Tone(125, 520, 0, 1.05, .20, Wave.Saw, 2); s.Ping(520,.54,.32,.18); s.Ping(780,.78,.35,.21); s.Ping(1170,1.04,.52,.27); }),
    ("12 System Shutdown.wav", 1.42, s => { s.Ping(980,0,.30,.22); s.Ping(650,.23,.32,.20); s.Tone(480, 95,.47,.82,.22,Wave.Saw,3); s.Noise(.55,.62,.04,120,1800); }),
    ("13 Task Complete.wav", .78, s => { s.Ping(520,0,.20,.20); s.Ping(690,.14,.22,.23); s.Ping(920,.29,.39,.29); s.Ping(1380,.43,.27,.12); }),
    ("14 Program Launch.wav", .68, s => { s.Noise(0,.23,.07,350,6200); s.Tone(240, 1320,.02,.47,.25,Wave.Sine,4); s.Ping(1640,.38,.24,.24); }),
    ("15 Window Open.wav", .42, s => { s.Tone(330, 1040,0,.31,.23,Wave.Triangle); s.Ping(1320,.20,.18,.19); }),
    ("16 Window Close.wav", .42, s => { s.Ping(1050,0,.15,.16); s.Tone(880, 260,.09,.29,.24,Wave.Triangle); }),
    ("17 Transition Shimmer.wav", 2.15, s => { s.Noise(.03,1.85,.075,900,10500); for(var i=0;i<11;i++) s.Tone(430+i*83,520+i*121,.05+i*.075,1.25,.035,Wave.Sine,7+i*.7); s.Ping(1480,1.58,.42,.19); }),
    ("18 Ambient Console.wav", 4.0, s => { s.Tone(92,96,0,3.85,.065,Wave.Sine,.35); s.Tone(184,181,0,3.85,.035,Wave.Sine,.22); for(var i=0;i<8;i++) s.Ping(630+(i*137)%680,.35+i*.46,.12,.045); }),
};

foreach (var sound in sounds)
{
    var synth = new Synth(sampleRate, sound.Seconds * profile.TimeScale,
        sound.Name.GetHashCode(StringComparison.Ordinal), profile);
    sound.Build(synth);
    synth.AddProfileCharacter();
    synth.Finish();
    Wav.Write(Path.Combine(output, sound.Name), synth.Samples, sampleRate);
    Console.WriteLine(sound.Name);
}

Console.WriteLine($"Generated {sounds.Length} original sounds in {output}");

enum Wave { Sine, Triangle, Square, Saw }

sealed record SoundProfile(double TimeScale, double PitchScale, double AttackScale,
    double ReleaseScale, double EchoSeconds, float EchoLevel, bool AddBass)
{
    public static readonly SoundProfile Set1 = new(1, 1, 1, 1, .036, .105f, false);
    public static readonly SoundProfile Set2 = new(1.85, .70, 2.8, 2.15, .075, .16f, true);
}

sealed class Synth(int rate, double seconds, int seed, SoundProfile profile)
{
    readonly Random random = new(seed);
    public float[] Samples { get; } = new float[(int)(rate * seconds)];

    public void Ping(double frequency, double start, double duration, double volume) =>
        Tone(frequency, frequency * .992, start, duration, volume, Wave.Sine, 0, .004, duration * .20);

    public void Tone(double fromHz, double toHz, double start, double duration, double volume,
        Wave wave, double vibratoHz = 0, double attack = .008, double release = .055)
    {
        fromHz *= profile.PitchScale;
        toHz *= profile.PitchScale;
        start *= profile.TimeScale;
        duration *= profile.TimeScale;
        attack *= profile.AttackScale;
        release *= profile.ReleaseScale;
        var first = Math.Max(0, (int)(start * rate));
        var count = Math.Min(Samples.Length - first, (int)(duration * rate));
        double phase = 0;
        for (var i = 0; i < count; i++)
        {
            var p = i / (double)Math.Max(1, count - 1);
            var hz = fromHz * Math.Pow(toHz / fromHz, p);
            if (vibratoHz > 0) hz *= 1 + .012 * Math.Sin(2 * Math.PI * vibratoHz * i / rate);
            phase += hz / rate;
            phase -= Math.Floor(phase);
            var value = wave switch
            {
                Wave.Sine => Math.Sin(2 * Math.PI * phase),
                Wave.Triangle => 1 - 4 * Math.Abs(phase - .5),
                Wave.Square => phase < .5 ? 1 : -1,
                _ => 2 * phase - 1
            };
            var envelope = Envelope(i / (double)rate, duration, attack, release);
            Samples[first + i] += (float)(value * envelope * volume);
        }
    }

    public void Noise(double start, double duration, double volume, double lowHz, double highHz)
    {
        start *= profile.TimeScale;
        duration *= profile.TimeScale;
        lowHz *= profile.PitchScale;
        highHz *= profile.PitchScale;
        var first = Math.Max(0, (int)(start * rate));
        var count = Math.Min(Samples.Length - first, (int)(duration * rate));
        var lowState = 0.0;
        var previous = 0.0;
        var lowAlpha = 1 - Math.Exp(-2 * Math.PI * highHz / rate);
        var highAlpha = Math.Exp(-2 * Math.PI * lowHz / rate);
        for (var i = 0; i < count; i++)
        {
            var white = random.NextDouble() * 2 - 1;
            lowState += lowAlpha * (white - lowState);
            var band = highAlpha * (previous + lowState - white);
            previous = band;
            var envelope = Envelope(i / (double)rate, duration, .025, .09);
            Samples[first + i] += (float)(band * envelope * volume);
        }
    }

    public void AddProfileCharacter()
    {
        if (!profile.AddBass) return;
        var duration = Samples.Length / (double)rate;
        var bassHz = 72 + Math.Abs(seed % 5) * 9;
        AddUnscaledTone(bassHz, bassHz * 1.035, 0, duration * .88, .115, .11, .42);
        AddUnscaledTone(bassHz * 2, bassHz * 1.92, duration * .08, duration * .64, .045, .16, .35);
    }

    public void Finish()
    {
        // A subtle short echo gives the pack a shared console-space character.
        var delay = (int)(rate * profile.EchoSeconds);
        for (var i = delay; i < Samples.Length; i++) Samples[i] += Samples[i-delay] * profile.EchoLevel;
        var peak = Samples.Max(x => Math.Abs(x));
        if (peak > .89f)
        {
            var scale = .89f / peak;
            for (var i = 0; i < Samples.Length; i++) Samples[i] *= scale;
        }
        var fade = Math.Min((int)(rate * .012), Samples.Length / 2);
        for (var i = 0; i < fade; i++) Samples[Samples.Length - 1 - i] *= i / (float)fade;
    }

    void AddUnscaledTone(double fromHz, double toHz, double start, double duration,
        double volume, double attack, double release)
    {
        var first = Math.Max(0, (int)(start * rate));
        var count = Math.Min(Samples.Length - first, (int)(duration * rate));
        double phase = 0;
        for (var i = 0; i < count; i++)
        {
            var p = i / (double)Math.Max(1, count - 1);
            var hz = fromHz * Math.Pow(toHz / fromHz, p);
            phase = (phase + hz / rate) % 1;
            var envelope = Envelope(i / (double)rate, duration, attack, release);
            Samples[first + i] += (float)(Math.Sin(2 * Math.PI * phase) * envelope * volume);
        }
    }

    static double Envelope(double time, double duration, double attack, double release)
    {
        var a = Math.Min(1, time / Math.Max(.001, attack));
        var r = Math.Min(1, (duration - time) / Math.Max(.001, release));
        return Math.Max(0, Math.Min(a, r)) * Math.Exp(-time / Math.Max(.05, duration * .82));
    }
}

static class Wav
{
    public static void Write(string path, float[] samples, int rate)
    {
        using var stream = File.Create(path);
        using var writer = new BinaryWriter(stream, Encoding.ASCII, false);
        const short channels = 1, bits = 16;
        var bytes = samples.Length * 2;
        writer.Write(Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + bytes);
        writer.Write(Encoding.ASCII.GetBytes("WAVEfmt ")); writer.Write(16);
        writer.Write((short)1); writer.Write(channels); writer.Write(rate);
        writer.Write(rate * channels * bits / 8); writer.Write((short)(channels * bits / 8)); writer.Write(bits);
        writer.Write(Encoding.ASCII.GetBytes("data")); writer.Write(bytes);
        foreach (var sample in samples) writer.Write((short)Math.Round(Math.Clamp(sample, -1, 1) * short.MaxValue));
    }
}
