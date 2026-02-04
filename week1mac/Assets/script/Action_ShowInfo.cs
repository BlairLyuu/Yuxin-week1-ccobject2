using UnityEngine;
using TMPro; // 记得引用 TextMeshPro

public class Action_ShowInfo : MonoBehaviour
{
    public GameObject infoPanel; // 通用的 Canvas Panel
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    
    public string myTitle;
    [TextArea] public string myDesc;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (infoPanel) infoPanel.SetActive(true);
            if (titleText) titleText.text = myTitle;
            if (descText) descText.text = myDesc;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (infoPanel) infoPanel.SetActive(false);
        }
    }
}