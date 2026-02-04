using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Action_RotateWorld : MonoBehaviour
{
    [Header("视觉对象")]
    public Transform visualBeam;

    [Header("接力设置 (关键)")]
    // ★ 拖入 下一个面 的光柱物体 (它身上必须挂着 DelayActivator)
    public DelayActivator nextFaceActivator;

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
        // 1. 锁定与准备
        GameManager.Instance.IsInteracting = true;
        GameManager.Instance.TogglePlayerControl(false);
        if (wideCamera) wideCamera.Priority = 20;

        // 2. 光柱消失动画
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

        // 3. 旋转世界
        GameObject hinge = new GameObject("TempHinge");
        hinge.transform.position = transform.position;
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
        worldRoot.SetParent(originalParent, true);
        Destroy(hinge);

        // 4. 收尾
        if (wideCamera) wideCamera.Priority = 0;
        GameManager.Instance.TogglePlayerControl(true);
        GameManager.Instance.IsInteracting = false;

        // 5. ★★★ 拍醒下一个面的计时器 ★★★
        // 旋转结束了，玩家到新面了，通知下一个光柱开始倒数
        if (nextFaceActivator != null)
        {
            nextFaceActivator.BeginTimer();
        }
    }
}