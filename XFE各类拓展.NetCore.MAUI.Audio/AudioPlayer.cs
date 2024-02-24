#if ANDROID
using Android.Media;
#endif

namespace XFE各类拓展.NetCore.MAUI.Audio;

public class AudioPlayer : IDisposable
{
#if ANDROID
    private bool disposedValue;
    private AudioTrack audioTrack;
    private bool isPlaying;

    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            if (value)
                StartPlayback();
            else
                StopPlayback();
        }
    }
    public int SampleRate { get; set; } = 44100;
    public ChannelOut ChannelConfig { get; set; } = ChannelOut.Mono;
    public Encoding AudioFormat { get; set; } = Encoding.Pcm16bit;
    public AudioUsageKind AudioUsage { get; set; } = AudioUsageKind.Media;
    public AudioContentType AudioContentType { get; set; } = AudioContentType.Music;
    public AudioTrackMode AudioTrackMode { get; set; } = AudioTrackMode.Stream;
#endif

    public void StartPlayback()
    {
#if ANDROID
        audioTrack.Play();
        isPlaying = true;
#endif
    }

    public void StopPlayback()
    {
#if ANDROID
        isPlaying = false;
        audioTrack.Stop();
#endif
    }

    public void PlayAudioData(byte[] audioData)
    {
#if ANDROID
        if (isPlaying && audioData is not null)
            audioTrack.Write(audioData, 0, audioData.Length, WriteMode.NonBlocking);
#endif
    }

#if ANDROID
    public void PlayAudioData(byte[] audioData, WriteMode writeMode = WriteMode.NonBlocking)
    {
        if (isPlaying && audioData is not null)
            audioTrack.Write(audioData, 0, audioData.Length, writeMode);
    }
#endif

    private void Initialize()
    {
#if ANDROID
        int bufferSizeInBytes = AudioTrack.GetMinBufferSize(SampleRate, ChannelConfig, AudioFormat);
        var audioAttributes = new AudioAttributes.Builder()
            ?.SetUsage(AudioUsage)
            ?.SetContentType(AudioContentType)
            ?.Build();
        audioTrack = new AudioTrack(
            audioAttributes,
            new AudioFormat.Builder()
                ?.SetEncoding(AudioFormat)
                ?.SetSampleRate(SampleRate)
                ?.SetChannelMask(ChannelConfig)
                ?.Build(),
            bufferSizeInBytes,
            AudioTrackMode,
            0);
#endif
    }

    public AudioPlayer() => Initialize();

#if ANDROID
    public AudioPlayer(int sampleRate = 44100, ChannelOut channelConfig = ChannelOut.Mono, Encoding audioFormat = Encoding.Pcm16bit, AudioUsageKind audioUsage = AudioUsageKind.Media, AudioContentType audioContentType = AudioContentType.Music, AudioTrackMode audioTrackMode = AudioTrackMode.Stream)
    {
        SampleRate = sampleRate;
        ChannelConfig = channelConfig;
        AudioFormat = audioFormat;
        AudioUsage = audioUsage;
        AudioContentType = audioContentType;
        AudioTrackMode = audioTrackMode;
        Initialize();
    }
#endif

    protected virtual void Dispose(bool disposing)
    {
#if ANDROID
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                audioTrack.Release();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
#endif
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~AudioPlayer()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
