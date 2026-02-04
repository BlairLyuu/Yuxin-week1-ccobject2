using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 对象")]
    public GameObject startScreen;      // 开始界面
    public GameObject hudPanel;         // 游戏内界面 (包含进度图)
    public GameObject endScreen;        // 结束界面
    public Image progressImage;         // 显示进度的 Image 组件
    public CanvasGroup fadePanel;       // 用于黑屏渐变的 Panel

    [Header("资源")]
    public Sprite[] progressSteps;      // 拖入 5 张进度图片 (0% -> 100%)

    [Header("核心对象")]
    public CinemachineCamera introCamera; // 开场上帝视角相机
    public GameObject player;             // 玩家物体

    // ★ 全局锁：当它为 true 时，禁止玩家进行任何新的交互
    // 防止玩家在旋转的时候又去按传送，导致逻辑爆炸
    public bool IsInteracting { get; set; } = false;

    void Awake() { Instance = this; }

    void Start()
    {
        Time.timeScale = 1;
        IsInteracting = false;

        // UI 初始化
        startScreen.SetActive(true);
        hudPanel.SetActive(false);
        endScreen.SetActive(false);
        if (fadePanel) fadePanel.alpha = 0;

        // 鼠标显示
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 锁定玩家移动
        TogglePlayerControl(false);

        // 开场相机最高优先级
        if (introCamera) introCamera.Priority = 99;

        // 初始化进度
        SetProgress(0);
    }

    // --- 流程控制 ---

    public void StartGame()
    {
        startScreen.SetActive(false);
        hudPanel.SetActive(true);

        // 镜头切换：Intro -> FPS
        if (introCamera) introCamera.Priority = 0;

        // 隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 延迟解锁玩家 (给镜头飞过去一点时间)
        Invoke(nameof(EnablePlayerDelay), 2.0f);
    }

    void EnablePlayerDelay() { TogglePlayerControl(true); }

    public void EndGame()
    {
        IsInteracting = true; // 锁定所有操作
        hudPanel.SetActive(false);
        endScreen.SetActive(true);
        
        // 释放鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        TogglePlayerControl(false);
    }

    // --- 辅助功能 ---

    // 切换进度图片 (Index: 0-4)
    public void SetProgress(int index)
    {
        if (progressImage && index < progressSteps.Length)
        {
            progressImage.sprite = progressSteps[index];
        }
    }

    // 开关玩家控制 (自动寻找常见脚本)
    public void TogglePlayerControl(bool state)
    {
        // 尝试寻找 Unity 新版 StarterAssets 的脚本
        var fpsScript = player.GetComponent<UnityEngine.MonoBehaviour>(); 
        // 注意：如果你知道具体脚本名(如 FirstPersonController)，最好显式转换
        // 这里只是为了通用性暂时这样做，建议把你的脚本名替换掉 MonoBehaviour
        
        if (fpsScript) fpsScript.enabled = state;
        
        // 同时也控制刚体，防止物理惯性
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb && !state) rb.linearVelocity = Vector3.zero;
    }
}