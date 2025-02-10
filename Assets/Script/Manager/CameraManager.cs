using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : GeziBehaviour<CameraManager>
{
    public float sensitivity = 2.0f; // ��ת���ж�
    public float verticalRotationLimit = 80.0f; // ��ֱ��ת����
    public Camera camera;
    public GameObject target;

    private float verticalRotation = 0.0f; // ��ǰ��ֱ��ת�Ƕ�
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // ����ˮƽ��ת
        camera.transform.eulerAngles += new Vector3(0, mouseX, 0);

        // ���´�ֱ��ת
        verticalRotation -= mouseY; // ע�������Ǽ�������Ϊ��������ƶ�ʱ����ֱ�Ƕ�Ӧ�ü���
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

        // Ӧ�ô�ֱ��ת
        camera.transform.localEulerAngles = new Vector3(verticalRotation, camera.transform.localEulerAngles.y, 0);
        //����ע�ӵ�λ��
        if (target != null)
        {
            target.transform.position = camera.transform.position + camera.transform.forward * 3;
        }
        //���������λ��
    }
    public static void SetPlayerView(PlayerManager player)
    {
        //target=chara.transform.GetChild(0).gameObject;
        Instance.target = player.focusPoint;
        Instance.camera.transform.position = player.head.transform.position;
        Instance.camera.transform.eulerAngles = player.head.transform.eulerAngles;
    }
}
