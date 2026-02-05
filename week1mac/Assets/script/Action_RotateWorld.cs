using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Action_RotateWorld : MonoBehaviour
{
    [Header("Visual Object")]
    public Transform visualBeam;
    public DelayActivator nextFaceActivator;

    [Header("Player Control")]
    public GameObject playerVisuals; // 玩家模型

    [Header("★ Key: Spawn Point")]
    public Transform targetSpawnPoint; // 落脚点

    [Header("Rotation Settings")]
    public Transform worldRoot;
    public CinemachineCamera wideCamera;
    public Vector3 axis = new Vector3(1, 0, 0);
    public float angle = 90f;
    public float duration = 1.5f;

    // ★★★ 老师添加的部分：单次锁 ★★★
    bool isAnimPlayed = false;

    public void StartRotation()
    {
        // 如果已经播过了，直接返回，不再执行
        if (isAnimPlayed) return;

        isAnimPlayed = true; // 标记为已播放
        StartCoroutine(RotationRoutine());
    }

    IEnumerator RotationRoutine()
    {
        // 1. --- Lock & Invisible ---
        GameManager.Instance.IsInteracting = true;
        GameManager.Instance.TogglePlayerControl(false);

        Rigidbody rb = GameManager.Instance.player.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        if (playerVisuals) playerVisuals.SetActive(false);
        if (wideCamera) wideCamera.Priority = 20;

        // 2. --- Beam Disappear ---
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

        // 3. --- Rotate World ---
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

        // 4. --- Restore Structure ---
        worldRoot.SetParent(originalParent, true);
        Destroy(hinge);

        if (wideCamera) wideCamera.Priority = 0;

        // 5. --- ★★★ 我们修复的：父子团圆传送 + 强制站立 ★★★ ---
        if (targetSpawnPoint != null)
        {
            GameObject parentObj = GameManager.Instance.player;

            CharacterController childCC = parentObj.GetComponentInChildren<CharacterController>();
            Rigidbody childRB = parentObj.GetComponentInChildren<Rigidbody>();

            if (childCC != null) childCC.enabled = false;
            if (childRB != null) childRB.isKinematic = true;

            // 移动父物体
            parentObj.transform.position = targetSpawnPoint.position;

            // ★ 强制站立：只取 Y 轴旋转，修正躺平问题
            float targetY = targetSpawnPoint.eulerAngles.y;
            parentObj.transform.rotation = Quaternion.Euler(0, targetY, 0);

            // 修正子物体位置
            if (childCC != null)
            {
                childCC.transform.localPosition = Vector3.zero;
                childCC.transform.localRotation = Quaternion.identity;
            }

            Transform capsuleTransform = parentObj.transform.Find("PlayerCapsule");
            if (capsuleTransform != null)
            {
                capsuleTransform.localPosition = Vector3.zero;
                capsuleTransform.localRotation = Quaternion.identity;
            }

            if (childCC != null) childCC.enabled = true;
            if (childRB != null) childRB.isKinematic = false;

            Debug.Log("传送完成 (已加锁，无法再次触发)");
        }

        // 6. --- Unlock ---
        if (playerVisuals) playerVisuals.SetActive(true);
        if (rb) rb.isKinematic = false;

        GameManager.Instance.TogglePlayerControl(true);
        GameManager.Instance.IsInteracting = false;

        if (nextFaceActivator != null) nextFaceActivator.BeginTimer();
    }
}