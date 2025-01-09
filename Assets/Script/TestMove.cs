using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown( KeyCode.H))
        {
            Debug.Log("°´¼ü°´ÏÂ"+ Application.isPlaying);
            GetComponent<MMD4MecanimModelImpl>().InitializeOnEditor(); 
        }
    }
}
