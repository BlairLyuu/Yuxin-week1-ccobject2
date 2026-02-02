using UnityEngine;
using Unity.Cinemachine; // 关键：Unity 6 新版引用名字变了

public class CameraTriggerZone : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("把你的广角相机拖到这里")]
    public CinemachineCamera targetCamera; // 关键：变量类型变了

    [Tooltip("触发时的优先级 (必须高于主相机，比如20)")]
    public int highPriority = 20;

    [Tooltip("离开时的优先级 (必须低于主相机，比如0)")]
    public int lowPriority = 0;

    [Header("调试")]
    [Tooltip("请确保你的玩家对象 Tag 是 Player")]
    public string playerTag = "Player";

    private void Start()
    {
        // 游戏开始时，先把广角相机设为低优先级，防止一开始就切过去
        if (targetCamera != null)
        {
            targetCamera.Priority = lowPriority;
        }
    }

    // 当玩家走进透明盒子
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (targetCamera != null)
            {
                targetCamera.Priority = highPriority;
                // Debug.Log("切换到广角！");
            }
        }
    }

    // 当玩家走出透明盒子
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (targetCamera != null)
            {
                targetCamera.Priority = lowPriority;
                // Debug.Log("还原视角");
            }
        }
    }
}
