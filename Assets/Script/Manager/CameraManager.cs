using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : GeziBehaviour<CameraManager>
{
    public float sensitivity = 2.0f; // 旋转敏感度
    public float verticalRotationLimit = 80.0f; // 垂直旋转限制
    public Camera camera;
    public GameObject target;

    private float verticalRotation = 0.0f; // 当前垂直旋转角度
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt)|| Input.GetKey(KeyCode.RightAlt))
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 更新水平旋转
        camera.transform.eulerAngles += new Vector3(0, mouseX, 0);

        // 更新垂直旋转
        verticalRotation -= mouseY; // 注意这里是减法，因为鼠标向上移动时，垂直角度应该减少
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

        // 应用垂直旋转
        camera.transform.localEulerAngles = new Vector3(verticalRotation, camera.transform.localEulerAngles.y, 0);
        if (target != null )
        {
            target.transform.position = camera.transform.position + camera.transform.forward * 3;
        }
    }
    public void SetPlayerView(GameObject chara )
    {
        target=chara.transform.GetChild(0).gameObject;
    }
}
