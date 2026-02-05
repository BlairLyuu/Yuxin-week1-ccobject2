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
        // ★★★ 间谍代码开始 ★★★
        Debug.Log("【Trigger测试】有东西进来了！名字叫：" + other.gameObject.name + " | 它的Tag是：" + other.tag);
        Debug.Log("【Trigger测试】当前的交互锁状态 IsInteracting = " + GameManager.Instance.IsInteracting);
        // ★★★ 间谍代码结束 ★★★

        if (other.CompareTag("Player") && !GameManager.Instance.IsInteracting)
        {
            Debug.Log("【Trigger测试】>>> 身份验证成功！显示按E提示 <<<"); // 确认成功
            isInZone = true;
            if (promptUI) promptUI.SetActive(true);
        }
        else
        {
            Debug.Log("【Trigger测试】>>> 拒绝访问！Tag不对 或 正在交互中 <<<"); // 确认失败原因
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