// 如果你用的是新版 Cinemachine (3.x)，请把下面这行换成 using Unity.Cinemachine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine;

public class MirrorInteract : MonoBehaviour
{
    [Header("关联对象")]
    public Transform targetApple;

    // 如果你用的是新版 Cinemachine (3.x)，请把下面这行换成 public CinemachineCamera mirrorCamera;
    public CinemachineCamera mirrorCamera;

    [Tooltip("把挂有 Canvas Group 组件的 UI 面板拖到这里")]
    public CanvasGroup clueUIGroup;

    [Header("参数设置")]
    public float rotateSpeed = 500f;
    public float uiDelay = 2.0f;
    public float fadeSpeed = 2.0f;   // 新增：UI 渐变的速度

    private bool isTriggered = false;

    void Start()
    {
        // 游戏开始时，确保 UI 是完全透明的，并且不能被点击
        if (clueUIGroup != null)
        {
            clueUIGroup.alpha = 0f;
            clueUIGroup.interactable = false;
            clueUIGroup.blocksRaycasts = false;
        }
        if (mirrorCamera != null) mirrorCamera.Priority = 0;
    }

    void OnMouseDrag()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

        if (targetApple != null)
        {
            targetApple.Rotate(0, -mouseX, 0, Space.World);
        }

        if (!isTriggered)
        {
            isTriggered = true;
            StartCoroutine(FocusAndFadeInUI());
        }
    }

    // 镜头切换与平滑出现 UI
    private IEnumerator FocusAndFadeInUI()
    {
        Debug.Log("镜头推近！");
        if (mirrorCamera != null) mirrorCamera.Priority = 10;

        yield return new WaitForSeconds(uiDelay);

        if (clueUIGroup != null)
        {
            Debug.Log("UI 开始平滑浮现！");
            // 逐渐增加透明度，直到完全显示 (alpha = 1)
            while (clueUIGroup.alpha < 1f)
            {
                clueUIGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null; // 等待下一帧
            }

            // UI 完全显现后，允许玩家点击上面的按钮
            clueUIGroup.interactable = true;
            clueUIGroup.blocksRaycasts = true;
        }
    }

    // ==========================================
    // 这个公开方法留给 UI 上的 "关闭" 按钮调用
    // 因为按钮不能直接调用协程，所以我们用这个普通方法做个“中转”
    // ==========================================
    public void OnCloseButtonClicked()
    {
        StartCoroutine(FadeOutAndReturn());
    }

    // 平滑消失 UI 并退回镜头
    private IEnumerator FadeOutAndReturn()
    {
        if (clueUIGroup != null)
        {
            // 一旦点击关闭，立刻禁止再次点击，防止重复触发
            clueUIGroup.interactable = false;
            clueUIGroup.blocksRaycasts = false;

            Debug.Log("UI 开始平滑消失！");
            // 逐渐减少透明度，直到完全透明 (alpha = 0)
            while (clueUIGroup.alpha > 0f)
            {
                clueUIGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        // UI 消失后，镜头退回主视角
        if (mirrorCamera != null) mirrorCamera.Priority = 0;

        // 重置状态，允许下次再次点击镜子
        isTriggered = false;
        Debug.Log("镜头退回主视角！");
    }
}