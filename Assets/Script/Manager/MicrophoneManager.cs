using Sirenix.OdinInspector;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    public KeyCode startKey = KeyCode.V; // ���¿ո����ʼ����
    public bool IsRecording { get; set; } = false;

    private string deviceName;
    private AudioClip audioClip;
    private AudioSource audioSource;
    //public int pos;
    public GameObject Player;
    public float limit;
    //��˷�����
    public float Gain;
    //У׼ģʽ
    void Start()
    {
        // ��ȡ���п��õ���˷��豸
        string[] devices = Microphone.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No microphone devices found!");
            return;
        }
        // ѡ���һ�����õ���˷��豸
        deviceName = devices[0];
        Debug.Log("Using microphone: " + deviceName);

        // ��ʼ�� AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        //audioSource.loop = false;
    }

    void Update()
    {
        // ��ⰴ��״̬
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
            //Debug.Log("��ǰ����Ϊ" + currentVoulme);
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
            // ����һ���µ� AudioClip ���洢��������
            AudioClip clip = AudioClip.Create("ReconstructedAudio", samples.Length, 1, SampleRate, false);
            Task.Run(() => ProcessAudio(samples));
            //Debug.Log("¼��1�����ݣ����з���" + samples.Count());
            audioClip = Microphone.Start(deviceName, true, second + 1, SampleRate);
            return;
        }
        else if (!IsRecording && Microphone.GetPosition(deviceName) > 0)
        {
            float[] samples = new float[SampleRate * second * audioClip.channels];
            audioClip.GetData(samples, 0);
            Microphone.End(deviceName);
            //Debug.Log("¼��ʣ�����ݣ����з���" + samples.Count());
            //PlayAudioFromSamples(samples);
            return;
        }
        //Debug.Log("��¼������");
    }
    //async Task ProcessSamples(AudioClip clip, float[] samples)
    //{
    //    try
    //    {

    //        Debug.Log("m4" + (DateTime.Now - now));

    //        //clip.SetData(samples.Select(x => x * Gain).ToArray(), 0);
    //        //��Ƶ����
    //        Debug.Log("׼����������" + DateTime.Now);
    //        Save($"temp.wav", clip);
    //        Debug.Log("m5" + (DateTime.Now - now));

    //        Debug.Log("��ʼ����" + DateTime.Now);
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

    //        Debug.Log("�������" + DateTime.Now);
    //        if (IsCalibrationMode)
    //        {

    //            // ������Ƶ
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
        //Debug.Log("��ʼ��������" + (DateTime.Now - now));

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
                //Debug.Log("����ffmpeg" + (DateTime.Now - now));
                process.Start();
                // ��AudioClip����д���׼����
                using (BinaryWriter writer = new BinaryWriter(process.StandardInput.BaseStream))
                {
                    for (int i = 0; i < audioData.Length; i++)
                    {
                        short sample = (short)(audioData[i] * 32768.0f);
                        writer.Write(sample);
                    }
                }
                // ʹ��MemoryStream���洢�������
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    // ͬ����ȡ���
                    while ((bytesRead = process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                    }
                    // �ȴ��������
                    process.WaitForExit();
                    Debug.Log("ffmpeg�������" + (DateTime.Now - now));
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
                        // ���ڴ���ת��ΪAudioClip
                        byte[] newAudioData = memoryStream.ToArray();
                        //File.WriteAllBytes("yaya.test",newAudioData);
                        Debug.LogError($"audioData����Ϊ{newAudioData.Count()}");
                        await NetCommand.SendVoiceToSelf(newAudioData);
                        Debug.Log("���ݷ������" + (DateTime.Now - now));
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
    #region �ļ�����


    /// <summary>
    /// ¼���ļ�����
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
        //����ͷ
        FileStream fs = CreateEmpty(filePath);
        //д��������
        ConvertAndWrite(fs, clip);
        //��д�������ļ�ͷ
        WriteHeader(fs, clip);
        fs.Flush();
        fs.Close();
    }
    /// <summary>
    /// ����ͷ
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
    /// д��Ƶ����
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
    /// ��д�������ļ�ͷ
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
        //���ڴ���һ��ʱ���ڵ���Ƶ��Ϣ
        float[] volumeData = new float[VOLUME_DATA_LENGTH];

        int offset;
        //��ȡ¼�Ƶ���Ƶ�Ŀ�ͷλ��
        offset = Microphone.GetPosition(deviceName) - VOLUME_DATA_LENGTH + 1;

        if (offset < 0)
        {
            return 0f;
        }

        //��ȡ����
        audioClip.GetData(volumeData, offset);

        //��������
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
    [Button("��ӡ��ֵ")]
    public void GetMaxVolume(AudioClip audioClip)
    {

        //���ڴ���һ��ʱ���ڵ���Ƶ��Ϣ
        float[] volumeData = new float[audioClip.samples];


        //��ȡ����
        audioClip.GetData(volumeData, 0);
        Debug.Log(volumeData.Max());
        Debug.Log(volumeData.Min());
        Debug.Log(volumeData.Average());
    }
}

