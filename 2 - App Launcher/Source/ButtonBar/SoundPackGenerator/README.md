# DVMS Console Sound Pack Generator

This small .NET 9 program procedurally creates the original **DVMS Console**
sound-effect pack. It uses synthesized oscillators and noise; it contains no
third-party samples and is not intended to reproduce any particular film or
television sound.

Run from this directory to reproduce either set:

```powershell
dotnet run -- "..\ButtonBar\Sounds\DVMS Console\Set 1" set1
dotnet run -- "..\ButtonBar\Sounds\DVMS Console\Set 2" set2
```

Set 1 is the original, compact console collection. Set 2 uses lower tuning,
slower envelopes, a synthesized bass foundation, longer timings, and a more
spacious echo. Both sets contain the same 18 named actions.

The generated WAV files are mono, 48 kHz, 16-bit PCM for fast desktop playback.
Changing a recipe in `Program.cs` and running the generator again replaces the
corresponding generated files.
