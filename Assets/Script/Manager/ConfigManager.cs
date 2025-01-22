using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfigManager : GeziBehaviour<ConfigManager>
{
    [Header("模板")]
    public GameObject template;
    private void Start()
    {
        foreach (Transform model in transform.GetChild(0))
        {
            model.gameObject.SetActive(false);
        }
    }
    [Button("生成新人物")]
    public void CreatModel(GameObject chara, GameObject binFile)
    {
        //关闭所有人物可见性
        foreach (Transform model in transform.GetChild(0))
        {
            model.gameObject.SetActive(false);
        }
        //根据模板生成新实例
        GameObject newModel = Instantiate(template, template.transform.parent);
        newModel.SetActive(true);
        newModel.name = chara.name;
        GameObject oldChara = newModel.transform.GetChild(0).gameObject;
        //替换模型
        GameObject newChara = Instantiate(chara, oldChara.transform.parent);
        newChara.name = chara.name + "模型";
        newChara.transform.position = oldChara.transform.position;
        newChara.transform.localScale = oldChara.transform.localScale;
        //装配模型上组组件
        var playerManager = newModel.GetComponent<PlayerManager>();
        newChara.transform.SetAsFirstSibling();
        playerManager.currentPlayerChara = Enum.Parse<Chara>(chara.name);
        //newChara.AddComponent<Animator>();
        var ikManager = newChara.AddComponent<IKManager>();
        ikManager.target = playerManager.focusPoint.transform;
        var mmdModel = newChara.AddComponent<MMD4MecanimModelImpl>();
        mmdModel.physicsEngine = MMD4MecanimModelImpl.PhysicsEngine.BulletPhysics;
        //配置语音
        FindComponentInChild(newModel, newChara.transform);

        if (ikManager != null)
            DestroyImmediate(oldChara);

        static void FindComponentInChild(GameObject currentModel, Transform currentTransform)
        {
            foreach (Transform item in currentTransform)
            {
                if (item.TryGetComponent(out SkinnedMeshRenderer renderer) && renderer.sharedMesh.blendShapeCount > 0)
                {
                    var keys = Enumerable.Range(0, renderer.sharedMesh.blendShapeCount)
                        .Select(i => renderer.sharedMesh.GetBlendShapeName(i))
                        .ToList();
                    var a = keys.IndexOf(keys.Where(key => key.Contains("あ")).OrderBy(key => key.Length).FirstOrDefault());
                    var e = keys.IndexOf(keys.Where(key => key.Contains("え")).OrderBy(key => key.Length).FirstOrDefault());
                    var i = keys.IndexOf(keys.Where(key => key.Contains("い")).OrderBy(key => key.Length).FirstOrDefault());
                    var o = keys.IndexOf(keys.Where(key => key.Contains("お")).OrderBy(key => key.Length).FirstOrDefault());
                    var u = keys.IndexOf(keys.Where(key => key.Contains("う")).OrderBy(key => key.Length).FirstOrDefault());
                    FaceManager faceManager = currentModel.GetComponent<FaceManager>();
                    faceManager.skinnedMeshRenderer = renderer;
                    faceManager.a = a;
                    faceManager.e = e;
                    faceManager.i = i;
                    faceManager.o = o;
                    faceManager.u = u;
                    Debug.LogWarning($"口型匹配结果a{a} e{e} i{i} o{o} u{u}");
                }
                else
                {
                    FindComponentInChild(currentModel, item);
                }
            }
        }
        //更新模型shader
    }
    [Button("切换当前人物")]
    public async void SelectModel(Chara chara)
    {
        //切换的是同个人物，不处理
        if (GameManager.PlayerChara == chara)
        {
            return;
        }
        PlayerManager oldModel = GameManager.CurrentConfigChara;
        PlayerManager newModel=null;
        foreach (Transform model in transform.GetChild(0))
        {
            bool isTarget = model.name == chara.ToString();
            if (isTarget)
            {
                newModel = model.GetComponent<PlayerManager>();
            }
        }

        var oldModelMaterials = oldModel?.charaMesh.SelectMany(mesh => mesh.materials).ToList();
        newModel.gameObject.SetActive(true);
        GameManager.PlayerChara = chara;
        GameManager.CurrentConfigChara = newModel.GetComponent<PlayerManager>();
        GameManager.SaveLocalUserData();
        await CustomThread.TimerAsync(2, (progress =>
        {
            //旧的缓慢消失
            if (oldModelMaterials!=null)
            {
                foreach (var material in oldModelMaterials)
                {
                    material.SetInt("_IsHide", 1);
                    material.SetFloat("_Dissolve", progress);
                };
            }
            //新的渐变生成
            var newModelMaterials = newModel.charaMesh.SelectMany(mesh => mesh.materials).ToList();
            foreach (var material in newModelMaterials)
            {
                material.SetInt("_IsHide", 0);
                material.SetFloat("_Dissolve", progress);
            };
        }));
        oldModel?.gameObject.SetActive(false);
    }
}
