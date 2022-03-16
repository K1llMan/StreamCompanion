using NAudio.Wave;

using NAudioPlayer.Classes;

namespace NAudioPlayer;

public class AudioPlayer
{
    #region Поля

    private WaveOutEvent outputDevice = new();
    private LinkedList<SongInfo> queue = new();
    private LinkedListNode<SongInfo> current;

    #endregion Поля

    #region Свойства

    public AudioPlayerConfig Config { get; }

    public SongInfo CurrengSong => current?.Value;

    #endregion Свойства

    #region События

    public event EventHandler<SongInfo> SongChanged;

    #endregion События

    #region Вспомогательные функции

    private void NormalizeVolume(AudioFileReader reader)
    {
        float max = 0;
        float[] buffer = new float[reader.WaveFormat.SampleRate];
        int read;
        do
        {
            read = reader.Read(buffer, 0, buffer.Length);
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
        Stop();

        AudioFileReader audioFile = new(fileName);
        NormalizeVolume(audioFile);

        outputDevice.Init(audioFile);
    }

    #endregion Вспомогательные функции

    #region Основные функции

    public void Play()
    {
        Task.Run(() => {
            SongChanged?.Invoke(this, CurrengSong);
            outputDevice.Play();

            while (outputDevice.PlaybackState == PlaybackState.Playing) {
                Thread.Sleep(1000);
            }
        });
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
        if (current.Next == null)
            return;
        
        current = current.Next;
        PlayFile(current.Value.FileName);
    }

    public void Previous()
    {
        if (current.Previous == null)
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
            InitSound(song.FileName);
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

    internal AudioPlayer(AudioPlayerConfig config)
    {
        Config = config;
        outputDevice.Volume = Config.Volume;
    }

    #endregion Основные функции
}