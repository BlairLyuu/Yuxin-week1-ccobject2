using UnityEngine;
using UnityEngine.UI;

public class ImageTrigger : MonoBehaviour
{
    [SerializeField] private GameObject imageUI;  // 拖入你的UI Image GameObject
    private bool hasTriggered = false;
    private bool isImageShowing = false;

    void Start()
    {
        // 确保UI一开始是隐藏的
        if (imageUI != null)
        {
            imageUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家，且之前没触发过
        if (other.CompareTag("Player") && !hasTriggered)
        {
            ShowImage();
            hasTriggered = true;  // 标记为已触发
        }
    }

    void Update()
    {
        // 如果图片正在显示，检测E键
        if (isImageShowing && Input.GetKeyDown(KeyCode.E))
        {
            HideImage();
        }
    }

    void ShowImage()
    {
        if (imageUI != null)
        {
            imageUI.SetActive(true);
            isImageShowing = true;
        }
    }

    void HideImage()
    {
        if (imageUI != null)
        {
            imageUI.SetActive(false);
            isImageShowing = false;
        }
    }
}