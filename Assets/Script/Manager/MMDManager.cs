using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMDManager : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer => transform.parent.GetComponent<FaceManager>().skinnedMeshRenderer;
    [Button("��ӡ��ǰ��������")]
    public void ShowCurrentFaceID()
    {
        List<int> blendShadpeID = new();
        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            if (skinnedMeshRenderer.GetBlendShapeWeight(i) != 0)
            {
                blendShadpeID.Add(i);
            }
        }
        string log = $"new(Chara.{transform.parent.name}, new() {{" + string.Join(", ", blendShadpeID)+"}}),";
        Debug.Log(log);
    }
}
