using UnityEngine;
using UnityEngine.Events;

public class KeyPressEventTrigger : MonoBehaviour
{
    [Header("设置按键")]
    [Tooltip("在下拉菜单中选择触发事件的按键")]
    public KeyCode triggerKey = KeyCode.E;

    [Header("事件触发器")]
    [Tooltip("当按键按下时，这里拖入的方法会被执行")]
    public UnityEvent OnKeyPressed;

    void Update()
    {
        // 检查指定的按键是否在当前帧被按下
        if (Input.GetKeyDown(triggerKey))
        {
            // 触发拖拽进去的所有事件
            OnKeyPressed?.Invoke();
        }
    }
}