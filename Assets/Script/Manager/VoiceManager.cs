using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    //加载指定角色语音音频
    public static List<AudioClip> LoadVoice(Chara chara)
    {
        //加载指定角色的语音
        var Voices = AssetBundleManager.LoadAll<AudioClip>(chara.ToString(), "Voice");
        return Voices.Where(voice => voice.name.StartsWith("V")).ToList();
    }

}
