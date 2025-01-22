using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoiceManager : GeziBehaviour<VoiceManager>
{
    public AudioSource audio;
    private void Awake() => audio = GetComponent<AudioSource>();
    //加载指定角色语音音频
    public static List<AudioClip> LoadVoice(Chara chara)
    {
        //加载指定角色的语音
        var Voices = AssetBundleManager.LoadAll<AudioClip>(chara.ToString(), "Voice");
        return Voices.Where(voice => voice.name.StartsWith("V")).ToList();
    }
    public static void PlayCardVoice(PlayCardVoiceType playCardVoiceType)
    {
        if (GameData.Instance.PlayCardVoice.ContainsKey(playCardVoiceType))
        {
            Instance.audio.clip = GameData.Instance.PlayCardVoice[playCardVoiceType];
            Instance.audio.Play();
        }
        else
        {
            Debug.LogError("无法查找到打出音频");
        }

    }
}
