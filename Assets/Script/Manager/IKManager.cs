using UnityEngine;

public class IKManager : MonoBehaviour
{
    public Transform target; // Ŀ��λ��

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
        // ����ͷ������Ŀ��λ��
        animator.SetLookAtPosition(target.position);
        // ����ͷ��Ȩ�أ�ֵ��Χ�� 0 �� 1
        animator.SetLookAtWeight(1.0f, 0.05f, 1.0f);
    }
}