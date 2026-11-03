using NAudio.Wave;

namespace DVMS.ButtonBar;

internal static class SoundService
{
    private static readonly object SyncRoot = new();
    private static readonly List<(WaveOutEvent Output, AudioFileReader Reader)> ActivePlayers = new();

    public static void Play(string soundFile, string soundDirectory)
    {
        if (string.IsNullOrWhiteSpace(soundFile)) return;

        var path = ResolvePath(soundFile, soundDirectory);
        if (!File.Exists(path))
        {
            MessageBox.Show($"The configured sound file was not found.\n\n{path}", "Sound file not found",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var reader = new AudioFileReader(path);
            var output = new WaveOutEvent();
            output.Init(reader);
            lock (SyncRoot) ActivePlayers.Add((output, reader));
            output.PlaybackStopped += (_, _) => Release(output, reader);
            output.Play();
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Button Bar could not play the sound.\n\n{exception.Message}",
                "Unable to play sound", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static string ResolvePath(string soundFile, string soundDirectory)
    {
        var expanded = Environment.ExpandEnvironmentVariables(soundFile);
        if (Path.IsPathRooted(expanded)) return expanded;
        var directory = Environment.ExpandEnvironmentVariables(soundDirectory);
        return string.IsNullOrWhiteSpace(directory) ? Path.GetFullPath(expanded) : Path.Combine(directory, expanded);
    }

    private static void Release(WaveOutEvent output, AudioFileReader reader)
    {
        lock (SyncRoot) ActivePlayers.RemoveAll(x => ReferenceEquals(x.Output, output));
        output.Dispose();
        reader.Dispose();
    }
}
