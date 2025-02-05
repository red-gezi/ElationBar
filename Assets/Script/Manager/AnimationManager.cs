using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationManager : GeziBehaviour<AnimationManager>
{

    public List<AnimatorData> charaActon;
    public static AnimationClip GetAnimationClip(ActionType actionType)
    {
        var clip = Instance.charaActon.FirstOrDefault(action => action.actionType == actionType);
        if (clip == null)
        {
            Debug.LogError($"未找到对应的动作类型: {actionType}");
        }
        return clip.animation; ;
    }
    [Button("刷新动画条目")]
    public void RefreshItem()
    {
        var allActionTypes = System.Enum.GetValues(typeof(ActionType));
        List<AnimatorData> tempList = new List<AnimatorData>();
        foreach (ActionType action in allActionTypes)
        {
            if (!charaActon.Exists(item => item.actionType == action))
            {
                tempList.Add(new AnimatorData { actionType = action, animation = null });
            }
        }
        charaActon.AddRange(tempList);
        // 按值排序
        charaActon.Sort((a, b) => a.actionType.CompareTo(b.actionType));
    }
}
