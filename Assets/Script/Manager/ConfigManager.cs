using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfigManager : GeziBehaviour<ConfigManager>
{
    [Header("ģ��")]
    public GameObject template;
    private void Start()
    {
        foreach (Transform model in transform.GetChild(0))
        {
            model.gameObject.SetActive(false);
        }
    }
    [Button("����������")]
    public void CreatModel(GameObject chara, TextAsset modelFile,BodyType bodyType)
    {
        //�ر���������ɼ���
        foreach (Transform model in transform.GetChild(0))
        {
            model.gameObject.SetActive(false);
        }
        //����ģ��������ʵ��
        GameObject newModel = Instantiate(template, template.transform.parent);
        newModel.SetActive(true);
        newModel.name = chara.name;
        GameObject oldChara = newModel.transform.GetChild(0).gameObject;
        //�滻ģ��
        GameObject newChara = Instantiate(chara, oldChara.transform.parent);
        newChara.name = chara.name + "ģ��";
        newChara.transform.position = oldChara.transform.position;
        newChara.transform.localScale = oldChara.transform.localScale;
        //װ��ģ���������
        var playerManager = newModel.GetComponent<PlayerManager>();
        newChara.transform.SetAsFirstSibling();
        playerManager.currentPlayerChara = Enum.Parse<Chara>(chara.name);
        Debug.LogWarning($"��ʼ����ͷ��IK");
        var ikManager = newChara.AddComponent<IKManager>();
        ikManager.target = playerManager.focusPoint.transform;
        Debug.LogWarning($"��ʼ����ģ������");
        var mmdModel = newChara.AddComponent<MMD4MecanimModelImpl>();
        mmdModel.modelFile= modelFile;
        mmdModel.physicsEngine = MMD4MecanimModelImpl.PhysicsEngine.BulletPhysics;
        //��������
        Debug.LogWarning($"��ʼ���ÿ���");
        //ѭ������ģ�ͣ���ȡ�沿�����Ͳ�����
        FindComponentInChild(newModel, newChara.transform, playerManager);
        if (ikManager != null)
            DestroyImmediate(oldChara);

        static void FindComponentInChild(GameObject currentModel, Transform currentTransform, PlayerManager playerManager)
        {
            foreach (Transform item in currentTransform)
            {
                if (item.TryGetComponent(out SkinnedMeshRenderer renderer)  )
                {
                    playerManager.charaMesh.Add(renderer);
                    //�����沿����
                    if (renderer.sharedMesh.blendShapeCount > 0)
                    {
                        var keys = Enumerable.Range(0, renderer.sharedMesh.blendShapeCount)
                                                .Select(i => renderer.sharedMesh.GetBlendShapeName(i))
                                                .ToList();
                        var a = keys.IndexOf(keys.Where(key => key.Contains("��")).OrderBy(key => key.Length).FirstOrDefault());
                        var e = keys.IndexOf(keys.Where(key => key.Contains("��")).OrderBy(key => key.Length).FirstOrDefault());
                        var i = keys.IndexOf(keys.Where(key => key.Contains("��")).OrderBy(key => key.Length).FirstOrDefault());
                        var o = keys.IndexOf(keys.Where(key => key.Contains("��")).OrderBy(key => key.Length).FirstOrDefault());
                        var u = keys.IndexOf(keys.Where(key => key.Contains("��")).OrderBy(key => key.Length).FirstOrDefault());
                        FaceManager faceManager = currentModel.GetComponent<FaceManager>();
                        faceManager.skinnedMeshRenderer = renderer;
                        faceManager.a = a;
                        faceManager.e = e;
                        faceManager.i = i;
                        faceManager.o = o;
                        faceManager.u = u;
                        Debug.LogWarning($"����ƥ����a{a} e{e} i{i} o{o} u{u}");
                    }
                }
                else
                {
                    FindComponentInChild(currentModel, item, playerManager);
                }
            }
        }
        //����ģ��shader
    }
    [Button("�л���ǰ����")]
    public async void SelectModel(Chara chara)
    {
        //�л�����ͬ�����������
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
            //�ɵĻ�����ʧ
            if (oldModelMaterials!=null)
            {
                foreach (var material in oldModelMaterials)
                {
                    material.SetInt("_IsHide", 1);
                    material.SetFloat("_Dissolve", progress);
                };
            }
            //�µĽ�������
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
