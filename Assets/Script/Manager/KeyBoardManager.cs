using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class KeyBoardManager : MonoBehaviour
{
    public KeyCode keyCode;
    Button button=>GetComponent<Button>();
    void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
            Debug.Log("按下了" + keyCode);
            button.onClick.Invoke();
        }
    }
}