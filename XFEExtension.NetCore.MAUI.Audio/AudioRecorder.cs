#if ANDROID
using Android.Media;
using Encoding = Android.Media.Encoding;
#endif

namespace XFEExtension.NetCore.MAUI.Audio;

public class AudioRecorder : IDisposable
{
#if ANDROID
    private AudioRecord audioRecord;
    private byte[] audioBuffer;
    private bool disposedValue;
    private Task? recordTask;
    private bool isRecording;

    public bool IsRecording
    {
        get => isRecording;
        set
        {
            if (value)
                StartRecording();
            else
                StopRecording();
        }
    }
    public int SampleRate { get; set; } = 44100;
    public ChannelIn ChannelConfig { get; set; } = ChannelIn.Mono;
    public Encoding AudioFormat { get; set; } = Encoding.Pcm16bit;
    public AudioSource AudioSource { get; set; } = AudioSource.Mic;

    public event EventHandler<byte[]>? BufferReceived;
#endif

    public void StartRecording()
    {
#if ANDROID
        if (!isRecording)
        {
            audioRecord.StartRecording();
            isRecording = true;
            recordTask = Task.Run(ReadAudio);
        }
        else
        {
            throw new Exception("当前正在录制！");
        }
#endif
    }

    public void StopRecording()
    {
#if ANDROID
        if (isRecording)
        {
            isRecording = false;
            audioRecord.Stop();
        }
        else
        {
            throw new Exception("录制已经停止，请勿重复停止");
        }
#endif
    }

#if ANDROID
    private async void ReadAudio()
    {
        while (isRecording)
        {
            int bytesRead = await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);

            if (bytesRead > 0)
            {
                BufferReceived?.Invoke(this, audioBuffer);
            }
        }
    }
#endif
    private void Initialize()
    {
#if ANDROID
        int bufferSize = AudioRecord.GetMinBufferSize(SampleRate, ChannelConfig, AudioFormat);
        audioBuffer = new byte[bufferSize];
        audioRecord = new AudioRecord(AudioSource, SampleRate, ChannelConfig, AudioFormat, bufferSize);
#endif
    }
    protected virtual void Dispose(bool disposing)
    {
#if ANDROID
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                audioRecord.Dispose();
                recordTask?.Dispose();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
#endif
    }

    // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    ~AudioRecorder()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    public AudioRecorder() => Initialize();
#if ANDROID
    public AudioRecorder(int sampleRate = 44100, ChannelIn channelConfig = ChannelIn.Mono, Encoding audioFormat = Encoding.Pcm16bit, AudioSource audioSource = AudioSource.Mic)
    {
        SampleRate = sampleRate;
        ChannelConfig = channelConfig;
        AudioFormat = audioFormat;
        AudioSource = audioSource;
        Initialize();
    }
#endif
}