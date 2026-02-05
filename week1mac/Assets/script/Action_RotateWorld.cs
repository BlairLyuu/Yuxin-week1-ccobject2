using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Action_RotateWorld : MonoBehaviour
{
    [Header("视觉对象")]
    public Transform visualBeam;
    public DelayActivator nextFaceActivator;

    [Header("玩家控制")]
    public GameObject playerVisuals; // 玩家的模型子物体

    [Header("★ 关键：落地点")]
    // 拖入你希望玩家旋转后“降落”的位置 (一个空物体)
    public Transform targetSpawnPoint;

    [Header("旋转设置")]
    public Transform worldRoot;
    public CinemachineCamera wideCamera;
    public Vector3 axis = new Vector3(1, 0, 0);
    public float angle = 90f;
    public float duration = 1.5f;

    public void StartRotation()
    {
        StartCoroutine(RotationRoutine());
    }

    IEnumerator RotationRoutine()
    {
        // 1. --- 锁定 & 隐身 ---
        GameManager.Instance.IsInteracting = true;
        GameManager.Instance.TogglePlayerControl(false);

        // 物理冻结 (防止乱动)
        Rigidbody rb = GameManager.Instance.player.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // 隐身 (防止穿帮)
        if (playerVisuals) playerVisuals.SetActive(false);

        if (wideCamera) wideCamera.Priority = 20;

        // 2. --- 光柱消失动画 ---
        if (visualBeam != null)
        {
            Vector3 originalScale = visualBeam.localScale;
            float t = 0f;
            while (t < 0.5f)
            {
                visualBeam.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / 0.5f);
                t += Time.deltaTime;
                yield return null;
            }
            visualBeam.gameObject.SetActive(false);
        }

        // 3. --- 旋转世界 ---
        GameObject hinge = new GameObject("TempHinge");
        hinge.transform.position = transform.position; // 以光柱为轴心旋转
        hinge.transform.rotation = Quaternion.identity;

        Transform originalParent = worldRoot.parent;
        worldRoot.SetParent(hinge.transform, true);

        Quaternion startRot = hinge.transform.rotation;
        Quaternion targetRot = startRot * Quaternion.AngleAxis(angle, axis);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);
            hinge.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hinge.transform.rotation = targetRot;

        // 4. --- 还原结构 ---
        worldRoot.SetParent(originalParent, true);
        Destroy(hinge);

        if (wideCamera) wideCamera.Priority = 0;

        // 5. --- ★★★ 关键修正：瞬移玩家 ★★★ ---
        if (targetSpawnPoint != null)
        {
            // 把玩家直接搬运到新地板的安全位置
            // 注意：要搬运 Player 最外层物体
            GameManager.Instance.player.transform.position = targetSpawnPoint.position;

            // 如果你想让玩家面向特定方向，也可以设置 rotation
            GameManager.Instance.player.transform.rotation = targetSpawnPoint.rotation;
        }

        // 6. --- 落地现身 ---
        if (playerVisuals) playerVisuals.SetActive(true);
        if (rb) rb.isKinematic = false;

        GameManager.Instance.TogglePlayerControl(true);
        GameManager.Instance.IsInteracting = false;

        // 激活下一个光柱
        if (nextFaceActivator != null) nextFaceActivator.BeginTimer();
    }
}