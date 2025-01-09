using UnityEngine;

public class IKManager : MonoBehaviour
{
    public Transform target; // 目标位置

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null ||  target == null) return;
        // 设置头部朝向目标位置
        animator.SetLookAtPosition(target.position);
        // 设置头部权重，值范围从 0 到 1
        animator.SetLookAtWeight(1.0f, 0.05f, 1.0f);
    }
}