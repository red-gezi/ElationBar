using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCameraManger : MonoBehaviour
{
    public Vector3 eular;
    // Start is called before the first frame update
    void Start()
    {
        eular = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = eular + new Vector3(Mathf.Sin(Time.time * 0.08f) * 5, Mathf.Cos(Time.time * 0.15f)) * 5;
    }
}
