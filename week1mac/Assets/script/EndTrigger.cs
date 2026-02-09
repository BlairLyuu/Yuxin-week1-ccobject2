using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [Header("设置")]
    // 这里直接拖入你的 EndPanel (或者 Canvas)
    public GameObject endPanel;

    private bool isGameEnded = false;

    void OnTriggerEnter(Collider other)
    {
        // 只有玩家能触发，且只触发一次
        if (other.CompareTag("Player") && !isGameEnded)
        {
            FinishGame();
        }
    }

    void FinishGame()
    {
        isGameEnded = true;
        Debug.Log("到达终点！");

        // 1. 打开结束界面
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        // 2. 停止玩家移动 (使用你现有的 GameManager)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePlayerControl(false);
            GameManager.Instance.IsInteracting = true;
        }

        // 3. 彻底暂停游戏时间 (可选，防止动画继续播放)
        Time.timeScale = 0;

        // 4. 把鼠标释放出来，让玩家能点界面上的按钮
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}