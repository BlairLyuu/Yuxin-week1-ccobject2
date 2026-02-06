using UnityEngine;

public class PlayerAnimDriver : MonoBehaviour
{
    private Animator anim;
    private CharacterController cc; // 假设你是用 CharacterController 移动的

    void Start()
    {
        anim = GetComponent<Animator>();
        // 尝试找父物体上的 CharacterController，因为脚本是挂在子物体模型上的
        cc = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (anim != null && cc != null)
        {
            // 获取当前水平移动速度 (忽略上下跳跃的速度)
            Vector3 horizontalVelocity = new Vector3(cc.velocity.x, 0, cc.velocity.z);
            float currentSpeed = horizontalVelocity.magnitude;

            // 把速度传给 Animator 的 "Speed" 参数
            anim.SetFloat("Speed", currentSpeed);
        }
    }
}