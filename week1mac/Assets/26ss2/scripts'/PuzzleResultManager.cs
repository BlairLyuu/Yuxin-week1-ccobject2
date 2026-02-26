using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleResultManager : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject popupPanel;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI buttonText;
    public Button actionButton;

    [Header("场景与组件引用")]
    public string nextSceneName = "NextScene";

    // ★ 就是这里！必须声明这个变量，报错才会消失
    public MirrorInteract_fix mirrorInteract;
    [SerializeField] PuzzleStarter puzzleStarter;
    void Start()
    {
        popupPanel.SetActive(false);
    }

    public void ShowResult(bool isSuccess)
    {
        popupPanel.SetActive(true);
        actionButton.onClick.RemoveAllListeners();

        if (isSuccess)
        {
            messageText.text = "congrats!";
            buttonText.text = "go";
            actionButton.onClick.AddListener(GoToNextScene);
        }
        else
        {
            messageText.text = "wrong Password";
            buttonText.text = "back";
            actionButton.onClick.AddListener(BackToAnchor0);
        }
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    void BackToAnchor0()
    {
        popupPanel.SetActive(false); // 先把弹窗隐藏掉
        puzzleStarter.SetPuzzleActive(false);
        // 调用你写好的平滑返回逻辑
        if (mirrorInteract != null)
        {
            mirrorInteract.ReturnToStart();
        }
        else
        {
            Debug.LogError("记得在 Inspector 面板里把带有 MirrorInteract_fix 的物体拖进来！");
        }
    }
}