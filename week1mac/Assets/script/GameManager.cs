using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 对象")]
    public GameObject startScreen;      // 开始界面
    public GameObject hudPanel;         // 游戏内界面
    public GameObject endScreen;        // 结束界面

    [Header("核心对象")]
    public CinemachineCamera introCamera; // 开场相机
    public GameObject player;             // 玩家物体 (用来控制位置/物理)

    // ★ 修改点1: 明确指定玩家控制脚本，防止抓错
    // 在 Inspector 里，把 Player 身上那个叫 "FirstPersonController" (或类似名字) 的脚本拖进来
    // 如果不知道是哪个，就把 Player 物体拖进来试试，Unity会自动找
    public MonoBehaviour playerControllerScript;

    public DelayActivator beamTimer;      // 拖入 WorldRoot

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

        // UI 初始化
        if (startScreen) startScreen.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);
        if (endScreen) endScreen.SetActive(false);

        // 隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 锁定玩家
        TogglePlayerControl(false);

        // 设置开场相机优先级
        if (introCamera) introCamera.Priority = 99;
    }

    // ★ 修改点2: Update 里的检测删掉了！
    // 现在的逻辑是：IntroEPress (Trigger) -> 检测按E -> 播放Timeline -> 呼叫 GameManager.StartGame()
    /* void Update()
    {
        if (!isGameStarted && Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }
    }
    */

    // 这个函数会被 IntroEPress 的事件调用
    public void StartGame()
    {
        if (isGameStarted) return; // 防止重复触发
        isGameStarted = true;

        Debug.Log("Game Started! 光柱计时器启动...");

        // 1. UI 切换
        if (startScreen) startScreen.SetActive(false);
        if (hudPanel) hudPanel.SetActive(true);

        // 2. 镜头切换 (配合 Timeline 结束)
        // 建议把这里的 Priority 改低，让 Timeline 播完后自动切回 Normal Camera
        if (introCamera) introCamera.Priority = 0;

        // 3. ★ 启动光柱倒计时 (关键！)
        if (beamTimer != null)
        {
            beamTimer.BeginTimer();
        }
        else
        {
            Debug.LogError("忘记拖入 Beam Timer (WorldRoot) 了！");
        }

        // 4. 延迟解锁玩家 
        // ★ 修改点3: 这里的 5.0f 应该改成你 Timeline 动画的时长
        // 比如你的动画是 3秒，这里就填 3.0f
        Invoke(nameof(EnablePlayerDelay), 3.0f);
    }

    void EnablePlayerDelay()
    {
        TogglePlayerControl(true);
    }

    public void EndGame()
    {
        IsInteracting = true;
        if (hudPanel) hudPanel.SetActive(false);
        if (endScreen) endScreen.SetActive(true);
        TogglePlayerControl(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TogglePlayerControl(bool state)
    {
        // 优先使用我们手动指定的脚本
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = state;
        }
        // 如果没指定，尝试自动找 (但可能会找错)
        else if (player != null)
        {
            // 尝试找 CharacterController (更通用的移动控制组件)
            var charController = player.GetComponent<CharacterController>();
            if (charController) charController.enabled = state;
        }

        // 物理锁定
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb && !state) rb.linearVelocity = Vector3.zero; // 强行停止惯性
        }
    }
}