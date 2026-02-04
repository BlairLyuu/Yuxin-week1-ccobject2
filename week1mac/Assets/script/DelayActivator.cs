using UnityEngine;
using System.Collections;

public class DelayActivator : MonoBehaviour
{
    [Header("设置")]
    public GameObject targetObject; // 拖入光柱触发器 (Portal_Edge)
    public float delayTime = 15f;   // 15秒
    public AudioSource soundFX;     // ★ 拖入挂了 AudioSource 的物体 (用于播放"崩"的一声)

    // 注意：去掉了 Start() 里的自动启动

    // ★ 这个函数由 GameManager 调用
    public void BeginTimer()
    {
        // 确保一开始是隐藏的
        if (targetObject != null) targetObject.SetActive(false);

        // 开始倒计时
        StartCoroutine(EnableRoutine());
    }

    IEnumerator EnableRoutine()
    {
        // 等待 15秒
        yield return new WaitForSeconds(delayTime);

        // 时间到：显示光柱
        if (targetObject != null) targetObject.SetActive(true);

        // ★ 播放音效
        if (soundFX != null) soundFX.Play();
    }
}