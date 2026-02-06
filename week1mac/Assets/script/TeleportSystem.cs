using UnityEngine;

public class TeleportSystem : MonoBehaviour
{
    [Header("传送设置")]
    public Transform targetPoint; // 目的地（把 B 点的 Transform 拖进来）
    public KeyCode teleportKey = KeyCode.Q; // 触发按键

    [Header("视觉反馈")]
    public bool isInsideTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;
            // 这里可以提示玩家：按 Q 传送
            Debug.Log("进入传送范围，请按 Q");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
        }
    }

    void Update()
    {
        // 如果玩家在范围内且按下了 Q
        if (isInsideTrigger && Input.GetKeyDown(teleportKey))
        {
            DoTeleport();
        }
    }

    void DoTeleport()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && targetPoint != null)
        {
            // 如果你有 CharacterController，必须先禁用它才能瞬移
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            // 传送位置和旋转
            player.transform.position = targetPoint.position;
            player.transform.rotation = targetPoint.rotation;

            if (cc) cc.enabled = true;

            Debug.Log("已传送到目的地！");
        }
    }
}