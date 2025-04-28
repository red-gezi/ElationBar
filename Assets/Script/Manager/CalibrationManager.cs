using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CalibrationManager : MonoBehaviour
{
    CharaManager playerManager;

    public Transform modelHeadPoint;
    public Transform modelLeftHandPoint;
    public Transform modelRightHandPoint;
    private void Awake() => playerManager = GetComponent<CharaManager>();
    [Header("校准双手坐标")]
    public Vector3 LeftHandPointPosBias;
    public Vector3 LeftHandPointEulaBias;
    public Vector3 RightHandPointPosBias;
    public Vector3 RightHandPointEulaBias;
    [Button("校准双手手点位")]
    public void CalibrationRightHand()
    {
        LeftHandPointPosBias = playerManager.leftHandPoint.position - modelLeftHandPoint.position;
        LeftHandPointEulaBias = modelLeftHandPoint.eulerAngles - playerManager.leftHandPoint.eulerAngles;
    }
    void Update()
    {
        if (modelHeadPoint != null)
        {
            playerManager.head.transform.position = modelHeadPoint.transform.position;

        }
        if (modelLeftHandPoint != null)
        {
            playerManager.leftHandPoint.position = modelLeftHandPoint.position + LeftHandPointPosBias;
            playerManager.leftHandPoint.rotation = modelLeftHandPoint.rotation * Quaternion.Euler(LeftHandPointEulaBias);
            //playerManager.leftHandPoint.transform.eulerAngles = modelLeftHandPoint.transform.eulerAngles + LeftHandPointEulaBias;
            modelLeftHandPoint.Cast<Transform>()
                .ToList()
                .ForEach(t=>Debug.DrawRay(t.position,t.up*0.5f));
            Debug.DrawRay(playerManager.leftHandPoint.position, playerManager.leftHandPoint.up * 0.5f, Color.red);
            
        }
        if (modelRightHandPoint != null)
        {
            playerManager.rightHandPoint.position = modelRightHandPoint.position + RightHandPointPosBias;
            playerManager.rightHandPoint.rotation = modelRightHandPoint.rotation * Quaternion.Euler(RightHandPointEulaBias);
            //playerManager.rightHandPoint.transform.eulerAngles = modelRightHandPoint.transform.eulerAngles + RightHandPointEulaBias;
        }

        
    }
}

