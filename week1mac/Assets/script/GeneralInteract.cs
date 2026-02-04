using UnityEngine;
using UnityEngine.Events; // 关键

public class GeneralInteract : MonoBehaviour
{
    [Header("设置")]
    public GameObject promptUI;     // "Press E" 的文字 UI (平时隐藏)
    public bool autoHideAfterUse = true; // 交互完是否隐藏光束/UI
    public GameObject visualBeam;   // 光束模型 (可选)

    [Header("绑定功能")]
    public UnityEvent onInteract;   // 在 Inspector 里拖入具体功能

    private bool isInZone = false;

    void Start()
    {
        if (promptUI) promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // 只有在没锁定的情况下才显示 UI
        if (other.CompareTag("Player") && !GameManager.Instance.IsInteracting)
        {
            isInZone = true;
            if (promptUI) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = false;
            if (promptUI) promptUI.SetActive(false);
        }
    }

    void Update()
    {
        // 核心判断：在圈内 + 按下E + 全局没在交互 + 游戏没暂停
        if (isInZone && Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.IsInteracting)
        {
            PerformInteract();
        }
    }

    void PerformInteract()
    {
        // 1. 关闭提示
        if (promptUI) promptUI.SetActive(false);
        if (autoHideAfterUse && visualBeam) visualBeam.SetActive(false);
        
        // 2. 执行绑定的所有功能
        onInteract.Invoke();

        // 3. 如果是一次性的，可以禁用自己
        // this.enabled = false; 
    }
}