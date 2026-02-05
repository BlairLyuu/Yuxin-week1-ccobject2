using UnityEngine;
using UnityEngine.Events;

public class GeneralInteract : MonoBehaviour
{
    [Header("设置")]
    public GameObject promptUI; // "Press E" 的 UI
    public KeyCode interactKey = KeyCode.E;

    Action_RotateWorld action_RotateWorld;
    // 内部状态
    private bool isInZone = false;

    void Start()
    {
        if (promptUI) promptUI.SetActive(false);

        action_RotateWorld =GetComponent<Action_RotateWorld>();
    }

    void Update()
    {
        // 检测条件：
        // 1. 人在区域内 (isInZone)
        // 2. 没在交互中 (!GameManager.IsInteracting)
        // 3. 按下了 E
        if (isInZone && !GameManager.Instance.IsInteracting && Input.GetKeyDown(interactKey))
        {
            Debug.Log("交互触发！");

            // 隐藏 UI，防止玩家以为还能按
            if (promptUI) promptUI.SetActive(false);

            // 执行绑定的事件 (旋转)
            action_RotateWorld.StartRotation();
        }
    }

    // 进门：开锁，亮 UI
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GameManager.Instance.IsInteracting)
        {
            isInZone = true;
            if (promptUI) promptUI.SetActive(true);
        }
    }

    // 出门：关锁，灭 UI (修复误触的关键)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = false;
            if (promptUI) promptUI.SetActive(false);
        }
    }
}