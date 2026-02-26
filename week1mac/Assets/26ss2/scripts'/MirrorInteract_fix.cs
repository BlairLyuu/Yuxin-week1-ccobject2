using UnityEngine;
using UnityEngine.UI; // 如果你的返回逻辑直接写在这里，可能需要UI命名空间

public class MirrorInteract_fix : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Transform camTrans;
    [SerializeField] Transform camAnchor_0, camAnchor_1;
    [SerializeField] float moveDuration = 2.0f; // 镜头移动时间

    [Header("UI Settings")]
    [SerializeField] CanvasGroup uiCanvasGroup; // 拖入你需要渐显的 UI 面板（需挂载 CanvasGroup 组件）
    [SerializeField] float uiFadeDuration = 2.0f; // UI 渐显时间

    private bool hasSwitched = false;
    private bool isMoving = false; // 防止在移动过程中重复点击

    void Start()
    {
        // 游戏开始时，确保 UI 是隐藏且不可交互的
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }
    }

    void Update()
    {
        // 如果已经切换了镜头，或者正在移动中，则不响应点击
        if (hasSwitched || isMoving) return;

        // 检测鼠标左键按下
        if (Input.GetMouseButtonDown(0))
        {
            PerformRaycast();
        }
    }

    void PerformRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            bool isHitMe = hit.transform == this.transform || hit.transform.IsChildOf(this.transform);

            if (isHitMe)
            {
                Debug.Log("检测到盒子，准备前往 Anchor_1");
                StartCoroutine(WaitMouseUpAndSwitch());
            }
        }
    }

    System.Collections.IEnumerator WaitMouseUpAndSwitch()
    {
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }

        SwitchCameras();
    }

    void SwitchCameras()
    {
        hasSwitched = true;
        // 开启平滑移动协程，从 anchor 0 移动到 1，并在结束时显示 UI (参数设为 true)
        StartCoroutine(MoveCameraRoutine(camAnchor_0, camAnchor_1, true));
    }

    // --- 新增：给返回按钮调用的公共方法 ---
    public void ReturnToStart()
    {
        if (isMoving) return; // 如果镜头还在动，不允许点击返回

        // 立即隐藏 UI (使其不可交互)
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }

        // 开启平滑移动协程，从 anchor 1 移动回 0，不显示 UI (参数设为 false)
        StartCoroutine(MoveCameraRoutine(camAnchor_1, camAnchor_0, false));
    }

    // --- 修改后的通用移动协程 ---
    System.Collections.IEnumerator MoveCameraRoutine(Transform startAnchor, Transform endAnchor, bool showUIAfterward)
    {
        isMoving = true;
        float elapsed = 0f;

        Vector3 startPos = startAnchor.position;
        Quaternion startRot = startAnchor.rotation;
        Vector3 endPos = endAnchor.position;
        Quaternion endRot = endAnchor.rotation;

        // 1. 执行镜头平滑移动
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            camTrans.position = Vector3.Lerp(startPos, endPos, smoothT);
            camTrans.rotation = Quaternion.Slerp(startRot, endRot, smoothT);

            yield return null;
        }

        // 确保完全对齐目标点
        camTrans.position = endPos;
        camTrans.rotation = endRot;

        // 2. 移动完成后的逻辑处理
        if (showUIAfterward)
        {
            // 如果是正向进入，开始渐显 UI
            StartCoroutine(FadeInUIRoutine());
        }
        else
        {
            // 如果是返回操作，重置状态，允许玩家再次点击镜子
            hasSwitched = false;
        }

        isMoving = false;
        Debug.Log("摄像机移动完成。");
    }

    // --- 新增：UI 渐显协程 ---
    System.Collections.IEnumerator FadeInUIRoutine()
    {
        if (uiCanvasGroup == null) yield break;

        float elapsed = 0f;
        float startAlpha = uiCanvasGroup.alpha;

        while (elapsed < uiFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / uiFadeDuration);
            uiCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, t);
            yield return null;
        }

        uiCanvasGroup.alpha = 1f;
        // 渐变结束后，允许玩家点击 UI（比如点击返回按钮）
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;
    }
}
