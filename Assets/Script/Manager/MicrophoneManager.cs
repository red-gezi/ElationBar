using Sirenix.OdinInspector;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    public KeyCode startKey = KeyCode.V; // 按下空格键开始发送
    public bool IsRecording { get; set; } = false;

    private string deviceName;
    private AudioClip audioClip;
    private AudioSource audioSource;
    //public int pos;
    public GameObject Player;
    public float limit;
    //麦克风增益
    public float Gain;
    //校准模式
    void Start()
    {
        // 获取所有可用的麦克风设备
        string[] devices = Microphone.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No microphone devices found!");
            return;
        }
        // 选择第一个可用的麦克风设备
        deviceName = devices[0];
        Debug.Log("Using microphone: " + deviceName);

        // 初始化 AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        //audioSource.loop = false;
    }

    void Update()
    {
        // 检测按键状态
        if (Input.GetKeyDown(startKey))
        {
            StartRecording();
            //UIManager.Instance.SetMicrophoneVolume(currentVoulme * Gain);
        }
        else if (Input.GetKeyUp(startKey))
        {
            StopRecording();
        }

        //pos = Microphone.GetPosition(deviceName);
        CollectionMicroPhoneData();
        float currentVoulme = GetMaxVolume();
        if (currentVoulme != 0)
        {
            //Debug.Log("当前音量为" + currentVoulme);
        }
    }
    //int SampleRate = AudioSettings.outputSampleRate/2;
    //int SampleRate = 44100;
    int SampleRate = 22050;
    int second = 3;
    void StartRecording()
    {
        Debug.Log(SampleRate);
        IsRecording = true;
        audioClip = Microphone.Start(deviceName, true, second + 1, SampleRate);
    }
    void StopRecording()
    {
        IsRecording = false;
    }
    DateTime now = DateTime.Now;
    void CollectionMicroPhoneData()
    {
        if (audioClip == null)
        {
            return;
        }
        if (IsRecording && Microphone.GetPosition(deviceName) >= SampleRate * second)
        {
            float[] samples = new float[SampleRate * second * audioClip.channels];
            now = DateTime.Now;
            audioClip.GetData(samples, 0);
            // 创建一个新的 AudioClip 来存储样本数据
            AudioClip clip = AudioClip.Create("ReconstructedAudio", samples.Length, 1, SampleRate, false);
            Task.Run(() => ProcessAudio(samples));
            //Debug.Log("录制1秒数据，进行发送" + samples.Count());
            audioClip = Microphone.Start(deviceName, true, second + 1, SampleRate);
            return;
        }
        else if (!IsRecording && Microphone.GetPosition(deviceName) > 0)
        {
            float[] samples = new float[SampleRate * second * audioClip.channels];
            audioClip.GetData(samples, 0);
            Microphone.End(deviceName);
            //Debug.Log("录制剩余数据，进行发送" + samples.Count());
            //PlayAudioFromSamples(samples);
            return;
        }
        //Debug.Log("无录制数据");
    }
    //async Task ProcessSamples(AudioClip clip, float[] samples)
    //{
    //    try
    //    {

    //        Debug.Log("m4" + (DateTime.Now - now));

    //        //clip.SetData(samples.Select(x => x * Gain).ToArray(), 0);
    //        //音频处理
    //        Debug.Log("准备保存数据" + DateTime.Now);
    //        Save($"temp.wav", clip);
    //        Debug.Log("m5" + (DateTime.Now - now));

    //        Debug.Log("开始处理" + DateTime.Now);
    //        string ffmpegPath = "ffmpeg";
    //        string arguments = $"-i temp.wav -y -af afftdn=nf=-20 -c:a libopus -b:a 128k -vbr on output2.opus";
    //        System.Diagnostics.ProcessStartInfo startInfo = new()
    //        {
    //            FileName = ffmpegPath,
    //            Arguments = arguments,
    //            RedirectStandardOutput = true,
    //            UseShellExecute = false,
    //            CreateNoWindow = true
    //        };
    //        using (System.Diagnostics.Process process = new() { StartInfo = startInfo })
    //        {
    //            process.Start();
    //            process.WaitForExit();
    //        }
    //        Debug.Log("m6" + (DateTime.Now - now));

    //        Debug.Log("处理完成" + DateTime.Now);
    //        if (IsCalibrationMode)
    //        {

    //            // 播放音频
    //            //AudioSource playbackSource = Player.GetComponent<AudioSource>();
    //            //playbackSource.clip = clip;
    //            //playbackSource.Play();
    //        }
    //        else
    //        {
    //            //_ = NetCommand.SendVoice(samples);
    //            //await NetCommand.SendVoice(File.ReadAllBytes("output2.opus"));
    //        }
    //        Debug.Log("m7" + (DateTime.Now - now));
    //    }
    //    catch (Exception e)
    //    {

    //        Debug.LogError(e.Message);
    //        Debug.LogError(e.StackTrace);
    //    }

    //}

    private async void ProcessAudio(float[] audioData)
    {
        //Debug.Log("开始处理数据" + (DateTime.Now - now));

        string ffmpegPath = "ffmpeg";
        //string arguments = "-y -af afftdn=nf=-20 -c:a pcm_s16le -ar  -ac 1 -f s16le pipe:1";
        //string arguments = $"-i temp.wav -y -af afftdn=nf=-20 -c:a libopus -b:a 128k -vbr on output2.opus";
        //string arguments = $"-y -f s16le -ar 44100 -ac 1 -i pipe:0 -af afftdn=nf=-20 -c:a libopus -b:a 128k -vbr on output22.opus";
        //string arguments = $"-y -f s16le -ar 44100 -ac 1 -i pipe:0 -af afftdn=nf=-20 -c:a libopus -b:a 128k -vbr on -f data pipe:1";
        //string arguments = $"-y -f s16le -ar 44100 -ac 1 -i pipe:0 -af afftdn=nf=-20 -c:a libopus -b:a 128k -vbr on -f opus pipe:1";
        string arguments = $"-y -f s16le -ar 22050 -ac 1 -i pipe:0 -af afftdn=nf=-20 -c:a libopus -b:a 96k -vbr on -f opus pipe:1";
        //string arguments = $"-y -f s16le -ar 16000 -ac 1 -i pipe:0 -af afftdn=nf=-20 -c:a libopus -b:a 32k -vbr off -f opus pipe:1";
        using (System.Diagnostics.Process process = new())
        {
            process.StartInfo.FileName = ffmpegPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            try
            {
                //Debug.Log("启动ffmpeg" + (DateTime.Now - now));
                process.Start();
                // 将AudioClip数据写入标准输入
                using (BinaryWriter writer = new BinaryWriter(process.StandardInput.BaseStream))
                {
                    for (int i = 0; i < audioData.Length; i++)
                    {
                        short sample = (short)(audioData[i] * 32768.0f);
                        writer.Write(sample);
                    }
                }
                // 使用MemoryStream来存储输出数据
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    // 同步读取输出
                    while ((bytesRead = process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                    }
                    // 等待进程完成
                    process.WaitForExit();
                    Debug.Log("ffmpeg运行完成" + (DateTime.Now - now));
                    string error = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        //Debug.LogError($"FFmpeg Error: {error}");
                    }
                    if (process.ExitCode != 0)
                    {
                        //Debug.LogError($"FFmpeg process exited with code {process.ExitCode}");
                    }
                    else
                    {
                        // 将内存流转换为AudioClip
                        byte[] newAudioData = memoryStream.ToArray();
                        //File.WriteAllBytes("yaya.test",newAudioData);
                        Debug.LogError($"audioData长度为{newAudioData.Count()}");
                        await NetCommand.SendVoiceToSelf(newAudioData);
                        Debug.Log("数据发送完毕" + (DateTime.Now - now));
                        //AudioClip audioClip = ConvertToAudioClip(newAudioData);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing audio: {ex.Message}");
            }
        }
        AudioClip ConvertToAudioClip(byte[] data)
        {
            int sampleRate = 44100;
            int channels = 2;
            int samples = data.Length / (sizeof(short) * channels);
            AudioClip audioClip = AudioClip.Create("ProcessedAudio", samples, channels, sampleRate, false);
            audioClip.SetData(ConvertBytesToFloats(data), 0);
            return audioClip;
        }
        float[] ConvertBytesToFloats(byte[] data)
        {
            float[] floats = new float[data.Length / sizeof(short)];
            for (int i = 0; i < floats.Length; i++)
            {
                short sample = BitConverter.ToInt16(data, i * sizeof(short));
                floats[i] = sample / 32768.0f;
            }
            return floats;
        }
    }
    #region 文件保存


    /// <summary>
    /// 录音文件保存
    /// </summary>
    const int HEADER_SIZE = 44;
    void Save(string fileName, AudioClip clip)
    {
        if (!fileName.ToLower().EndsWith(".wav"))
        {
            fileName += ".wav";
        }
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        if (Directory.Exists(filePath))
        {
            Directory.Delete(filePath, true);
        }
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        //Debug.Log(filePath);
        //创建头
        FileStream fs = CreateEmpty(filePath);
        //写语音数据
        ConvertAndWrite(fs, clip);
        //重写真正的文件头
        WriteHeader(fs, clip);
        fs.Flush();
        fs.Close();
    }
    /// <summary>
    /// 创建头
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    FileStream CreateEmpty(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Create);
        byte emptyByte = new byte();
        for (int i = 0; i < HEADER_SIZE; i++)
        {
            fileStream.WriteByte(emptyByte);
        }
        return fileStream;
    }
    /// <summary>
    /// 写音频数据
    /// </summary>
    /// <param name="fileSteam"></param>
    /// <param name="clip"></param>
    void ConvertAndWrite(FileStream fileSteam, AudioClip clip)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);
        Int16[] intData = new Int16[samples.Length];
        Byte[] bytesData = new Byte[samples.Length * 2];
        int rescaleFactor = 32767;
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArray = new byte[2];
            byteArray = BitConverter.GetBytes(intData[i]);
            byteArray.CopyTo(bytesData, i * 2);

        }
        fileSteam.Write(bytesData, 0, bytesData.Length);

    }
    /// <summary>
    /// 重写真正的文件头
    /// </summary>
    /// <param name="fileStream"></param>
    /// <param name="clip"></param>
    void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;
        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        Byte[] audioFormat = BitConverter.GetBytes(1);
        fileStream.Write(audioFormat, 0, 2);


        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);
        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);
        Byte[] byRate = BitConverter.GetBytes(hz * channels * 2);
        fileStream.Write(byRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
    }

    #endregion
    private float GetMaxVolume()
    {
        if (audioClip == null)
        {
            return 0;
        }
        float maxVolume = 0f;
        int VOLUME_DATA_LENGTH = 128;
        //用于储存一段时间内的音频信息
        float[] volumeData = new float[VOLUME_DATA_LENGTH];

        int offset;
        //获取录制的音频的开头位置
        offset = Microphone.GetPosition(deviceName) - VOLUME_DATA_LENGTH + 1;

        if (offset < 0)
        {
            return 0f;
        }

        //获取数据
        audioClip.GetData(volumeData, offset);

        //解析数据
        for (int i = 0; i < VOLUME_DATA_LENGTH; i++)
        {
            float tempVolume = volumeData[i];
            if (tempVolume > maxVolume)
            {
                maxVolume = tempVolume;
            }
        }

        return maxVolume;
    }
    [Button("打印数值")]
    public void GetMaxVolume(AudioClip audioClip)
    {

        //用于储存一段时间内的音频信息
        float[] volumeData = new float[audioClip.samples];


        //获取数据
        audioClip.GetData(volumeData, 0);
        Debug.Log(volumeData.Max());
        Debug.Log(volumeData.Min());
        Debug.Log(volumeData.Average());
    }
}

