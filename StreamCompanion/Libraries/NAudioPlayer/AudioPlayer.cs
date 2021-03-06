using System.Diagnostics;

using NAudio.Wave;

using NAudioPlayer.Classes;
using NAudioPlayer.Interfaces;

namespace NAudioPlayer;

public class AudioPlayer
{
    #region Поля

    private WaveOutEvent outputDevice = new();
    private WaveChannel32? stream;
    private LinkedList<SongInfo> queue = new();
    private LinkedListNode<SongInfo>? current;

    #endregion Поля

    #region Свойства

    public AudioPlayerConfig Config { get; }

    public SongInfo? CurrengSong => current?.Value;

    public List<ISongProvider> Providers { get; internal set; }

    #endregion Свойства

    #region События

    public event EventHandler<SongInfo> SongChanged;

    #endregion События

    #region Вспомогательные функции

    private string ChangeExtension(string fileName, string extension)
    {
        return Path.Combine(Path.GetDirectoryName(fileName), $"{Path.GetFileNameWithoutExtension(fileName)}.{extension}");
    }

    private void ConvertToMp3(SongInfo info)
    {
        if (string.IsNullOrEmpty(Config.FFMpegPath))
            return;

        string from = info.FileName;
        string temp = ChangeExtension(from, "tmp");
        if (File.Exists(temp))
            File.Delete(temp);

        File.Move(from, temp);

        string to = ChangeExtension(info.FileName, "mp3");

        Process process = new(){
            StartInfo = new ProcessStartInfo {
                FileName = Path.Combine(Config.FFMpegPath, "ffmpeg.exe"),
                Arguments = $"-i \"{temp}\" -codec:a libmp3lame -qscale:a 2 -y \"{to}\"",
                RedirectStandardOutput = true
            }
        };

        process.Start();
        process?.WaitForExit();
        File.Delete(temp);

        info.FileName = to;
    }

    private void NormalizeVolume(WaveChannel32 reader)
    {
        ISampleProvider? provider = reader.ToSampleProvider();

        float max = 0;
        float[] buffer = new float[reader.WaveFormat.SampleRate];

        int read;
        do
        {
            read = provider.Read(buffer, 0, buffer.Length);
            for (int n = 0; n < read; n++)
            {
                float abs = Math.Abs(buffer[n]);
                if (abs > max)
                    max = abs;
            }
        } while (read > 0);

        if (max == 0 || max > 1.0f)
            throw new InvalidOperationException("File cannot be normalized");

        // rewind and amplify
        reader.Position = 0;
        reader.Volume = 1.0f / max;
    }

    private void InitSound(string fileName)
    {
        WaveStream audioFile = new MediaFoundationReader(fileName);
        stream = new(audioFile) {
            PadWithZeroes = false
        };

        NormalizeVolume(stream);

        if (!IsSongEnded())
            Stop();

        outputDevice.Init(audioFile);
    }

    private bool IsSongEnded()
    {
        return outputDevice.PlaybackState == PlaybackState.Stopped;
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public void Play()
    {
        if (stream == null)
            return;

        Task.Run(() => {
            SongChanged?.Invoke(this, CurrengSong);
            outputDevice.Play();

            while (outputDevice.PlaybackState == PlaybackState.Playing) {
                Thread.Sleep(1000);
            }

            if (IsSongEnded())
                Next();
        });
    }

    private void OutputDeviceOnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void Pause()
    {
        outputDevice.Pause();
    }

    public void Stop()
    {
        outputDevice.Stop();
    }

    public void Next()
    {
        if (current?.Next == null)
            return;
        
        current = current.Next;
        PlayFile(current.Value.FileName);
    }

    public void Previous()
    {
        if (current?.Previous == null)
            return;
        
        current = current.Previous;
        PlayFile(current.Value.FileName);
    }

    public void Add(SongInfo song)
    {
        queue.AddLast(song);

        if (queue.Count == 1)
        {
            current = queue.First;
            PlayFile(song.FileName);
        }
        else if (current != null && song == queue.Last?.Value && IsSongEnded())
        {
            current = queue.Last;
            PlayFile(song.FileName);
        }
    }

    public void Remove(SongInfo song)
    {
        if (current.Value == song)
            current = current.Next;

        queue.Remove(song);
    }

    public void Volume(float volume)
    {
        volume = Math.Max(Math.Min(volume, 1), 0);

        Config.Volume = volume;
        outputDevice.Volume = Config.Volume;
    }

    public void PlayFile(string fileName)
    {
        InitSound(fileName);
        Play();
    }

    public void AddFromProvider(string url)
    {
        url = url.Trim();

        ISongProvider provider = Providers?
            .FirstOrDefault(p => p.IsCorrectUrl(url));

        if (provider == null)
            return;

        SongInfo info = provider.GetSong(Config.CachePath, url);
        if (info == null)
            return;

        Add(info);
    }

    internal AudioPlayer(AudioPlayerConfig config)
    {
        Config = config;
        outputDevice.Volume = Config.Volume;
    }

    #endregion Основные функции
}