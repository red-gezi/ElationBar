using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CalibrationManager : MonoBehaviour
{
    public GameObject modelHeadPoint;

    public GameObject modelLeftHandPoint;
    public GameObject modelRightHandPoint;
    PlayerManager playerManager;

    private void Awake() => playerManager = GetComponent<PlayerManager>();
    [Header("校准双手坐标")]
    public Vector3 LeftHandPointPosBias;
    public Vector3 LeftHandPointEulaBias;
    public Vector3 RightHandPointPosBias;
    public Vector3 RightHandPointEulaBias;
    [Button("校准双手手点位")]
    public void CalibrationRightHand()
    {
        LeftHandPointPosBias = playerManager.leftHand.transform.position - modelLeftHandPoint.transform.position;
        LeftHandPointEulaBias = modelLeftHandPoint.transform.eulerAngles - playerManager.leftHand.transform.eulerAngles;
    }
    void Update()
    {
        

        if (modelHeadPoint!=null)
        {
            playerManager.head.transform.position = modelHeadPoint.transform.position;

        }
        if (modelLeftHandPoint != null)
        {
            playerManager.leftHand.transform.position = modelLeftHandPoint.transform.position + LeftHandPointPosBias;
            playerManager.leftHand.transform.eulerAngles = modelLeftHandPoint.transform.eulerAngles + LeftHandPointEulaBias;
        }
        if (modelRightHandPoint != null)
        {
            playerManager.rightHand.transform.position = modelRightHandPoint.transform.position + RightHandPointPosBias;
            playerManager.rightHand.transform.eulerAngles = modelRightHandPoint.transform.eulerAngles + RightHandPointEulaBias;
        }
    }
}
