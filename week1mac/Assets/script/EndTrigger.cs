using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [Header("设置")]
    public GameObject endCanvas; // 拖入你的 EndCanvas
    public bool isGameEnded = false;

    void OnTriggerEnter(Collider other)
    {
        // 确保只有玩家碰到且还没结束过
        if (other.CompareTag("Player") && !isGameEnded)
        {
            ShowEndUI();
        }
    }

    void ShowEndUI()
    {
        isGameEnded = true;

        // 1. 显示图片
        if (endCanvas != null)
        {
            endCanvas.SetActive(true);
        }

        // 2. 锁定玩家控制 (引用你的 GameManager)
        GameManager.Instance.TogglePlayerControl(false);
        GameManager.Instance.IsInteracting = true;

        // 3. 释放鼠标（方便玩家点退出，可选）
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("游戏结束！");
    }
}