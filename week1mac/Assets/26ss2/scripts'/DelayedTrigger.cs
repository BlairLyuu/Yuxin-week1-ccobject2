using UnityEngine;
using UnityEngine.Events; // 必须引入这一行才能用面板事件
using System.Collections;

public class DelayedTrigger : MonoBehaviour
{
    [Tooltip("需要等待多少秒？")]
    public float delaySeconds = 2.5f;

    [Tooltip("时间到了之后，你要触发什么事情？(可以在面板里随便加)")]
    public UnityEvent onDelayComplete;

    // 这个方法留给其他机关（比如拉墙）去调用
    public void StartDelayTimer()
    {
        StartCoroutine(WaitAndTrigger());
    }

    private IEnumerator WaitAndTrigger()
    {
        // 1. 死等设定的秒数
        yield return new WaitForSeconds(delaySeconds);

        // 2. 时间到了，把面板里挂着的所有操作一口气全执行了！
        if (onDelayComplete != null)
        {
            onDelayComplete.Invoke();
            Debug.Log("延迟时间到！触发了挂载的所有事件！");
        }
    }
}