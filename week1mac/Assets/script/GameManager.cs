using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 对象")]
    public GameObject startScreen;      // 开始界面 (包含 "Press E to Start" 文字)
    public GameObject hudPanel;         // 游戏内界面
    public GameObject endScreen;        // 结束界面

    [Header("核心对象")]
    public CinemachineCamera introCamera; // 开场相机
    public GameObject player;             // 玩家
    public DelayActivator beamTimer;      // ★ 拖入挂了 DelayActivator 的物体 (WorldRoot)

    [Header("状态")]
    public bool isGameStarted = false;
    public bool IsInteracting { get; set; } = false; // 全局交互锁

    void Awake() { Instance = this; }

    void Start()
    {
        // 初始化状态
        isGameStarted = false;
        IsInteracting = false;
        Time.timeScale = 1;

        // UI & 鼠标
        startScreen.SetActive(true);
        hudPanel.SetActive(false);
        endScreen.SetActive(false);

        // 隐藏鼠标 (因为我们要用 E 键开始，不需要鼠标点)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 锁定玩家
        TogglePlayerControl(false);

        // 相机
        if (introCamera) introCamera.Priority = 99;
    }

    void Update()
    {
        // ★ 核心逻辑：如果在主菜单，并且按下了 E
        if (!isGameStarted && Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        isGameStarted = true;

        // 1. UI 切换
        startScreen.SetActive(false);
        hudPanel.SetActive(true);

        // 2. 镜头切换 (Intro -> FPS)
        if (introCamera) introCamera.Priority = 0;

        // 3. ★ 启动光柱倒计时 (关键！)
        if (beamTimer != null) beamTimer.BeginTimer();

        // 4. 延迟解锁玩家 (给镜头飞过去一点时间)
        Invoke(nameof(EnablePlayerDelay), 2.0f);
    }

    void EnablePlayerDelay() { TogglePlayerControl(true); }

    public void EndGame()
    {
        IsInteracting = true;
        hudPanel.SetActive(false);
        endScreen.SetActive(true);
        TogglePlayerControl(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TogglePlayerControl(bool state)
    {
        var fpsScript = player.GetComponent<UnityEngine.MonoBehaviour>();
        if (fpsScript) fpsScript.enabled = state;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb && !state) rb.linearVelocity = Vector3.zero;
    }
}