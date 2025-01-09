using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await AssetBundleManager.Init("PC_Release", true);
        Debug.LogWarning("重新载入完成");
        SceneManager.LoadScene("2_Game");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
