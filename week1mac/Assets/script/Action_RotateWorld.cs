using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Action_RotateWorld : MonoBehaviour
{
    public Transform worldRoot;
    public CinemachineCamera wideCamera; // 广角相机
    public Vector3 axis = new Vector3(1, 0, 0); // 轴向
    public float angle = 90f; // 角度
    public float duration = 1.5f;

    public void StartRotation()
    {
        StartCoroutine(RotationRoutine());
    }

    IEnumerator RotationRoutine()
    {
        // ★ 上锁
        GameManager.Instance.IsInteracting = true;
        GameManager.Instance.TogglePlayerControl(false); // 定身玩家

        // 1. 切换广角
        if (wideCamera) wideCamera.Priority = 20;

        // 2. 创建临时铰链
        GameObject hinge = new GameObject("TempHinge");
        hinge.transform.position = transform.position; // 轴心在 Trigger 处
        hinge.transform.rotation = Quaternion.identity;

        Transform originalParent = worldRoot.parent;
        worldRoot.SetParent(hinge.transform, true);

        // 3. 执行旋转
        Quaternion startRot = hinge.transform.rotation;
        Quaternion targetRot = startRot * Quaternion.AngleAxis(angle, axis);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // SmoothStep 曲线
            hinge.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hinge.transform.rotation = targetRot;

        // 4. 还原结构
        worldRoot.SetParent(originalParent, true);
        Destroy(hinge);

        // 5. 还原状态
        if (wideCamera) wideCamera.Priority = 0;
        
        // ★ 解锁
        GameManager.Instance.TogglePlayerControl(true);
        GameManager.Instance.IsInteracting = false;
    }
}