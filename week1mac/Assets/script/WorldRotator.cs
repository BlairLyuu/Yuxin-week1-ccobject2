using UnityEngine;
using System.Collections;

public class WorldRotator : MonoBehaviour
{
    [Header("核心设置")]
    public Transform worldRoot;   // 拖入那个包含所有环境的父物体
    public Transform player;      // 拖入你的 Player 玩家物体
    
    [Header("旋转参数")]
    public Vector3 rotationAxis = new Vector3(1, 0, 0); // 绕X轴转(前后) 还是 Z轴转(左右)
    public float angle = 90f;     // 转90度
    public float duration = 1.0f; // 旋转耗时

    private bool isRotating = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRotating)
        {
            StartCoroutine(RotateWorldSequence());
        }
    }

    IEnumerator RotateWorldSequence()
    {
        isRotating = true;

        // --- 关键步骤 1: 锁定玩家 ---
        // 把玩家临时变成 WorldRoot 的子物体
        // 这样当 WorldRoot 转动时，玩家会跟着一起转动位置，保持相对静止
        // 这就避免了玩家在倾斜的地面上滑落的问题
        Transform originalParent = player.parent;
        player.SetParent(worldRoot);

        // --- 关键步骤 2: 计算旋转 ---
        Quaternion startRot = worldRoot.rotation;
        Quaternion targetRot = startRot * Quaternion.AngleAxis(angle, rotationAxis);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 平滑旋转世界
            worldRoot.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保角度精确
        worldRoot.rotation = targetRot;

        // --- 关键步骤 3: 解锁玩家 ---
        // 旋转结束，把玩家恢复自由身（取消父子关系）
        // 这样玩家又可以继续基于新的水平面行走了
        player.SetParent(originalParent);
        
        // 可选：为了防止物理惯性，可以重置一下玩家的速度
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if(rb != null) rb.linearVelocity = Vector3.zero; // Unity 6 写法，旧版用 velocity

        isRotating = false;
    }
}