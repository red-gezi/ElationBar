using System.Collections.Generic;
using UnityEngine;
public class GameData : GeziBehaviour<GameData>
{
    public Dictionary<PlayCardVoiceType, AudioClip> PlayCardVoice;
    public record FaceData(Chara CurrentChara, List<int> KeyIndex);
    //按照喜、怒、哀、惧的顺序添加表情
    public static List<FaceData> FaceDatas { get; set; } = new List<FaceData>()
    {
       new(Chara.砂金, new() { 8, 16, 19, 44, 58, 59 }),
       new(Chara.砂金, new() { 1, 8, 61 }),
       new(Chara.砂金, new() { 7, 56 }),
       new(Chara.砂金, new() { 6, 48, 50, 59}),

       new(Chara.黑天鹅, new() { 13, 20, 51 }),
       new(Chara.黑天鹅, new() { 0, 1, 24, 37}),
       new(Chara.黑天鹅, new() { 4, 9, 44, 67 }),
       new(Chara.黑天鹅, new() { 23, 34, 40 }),

    };
}
