using UnityEngine;

public class GeziBehaviour<T> : MonoBehaviour where T : GeziBehaviour<T>
{
    public static T Instance;
    private void Awake()
    {
        Instance = this as T;
    }
}
