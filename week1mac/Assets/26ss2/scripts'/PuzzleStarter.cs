using UnityEngine;
using UnityEngine.UI;

public class PuzzleStarter : MonoBehaviour
{
    [Header("UI 引用")]
    [Tooltip("拖入和 MirrorInteract_fix 里同一个 CanvasGroup")]
    public CanvasGroup preUICanvasGroup;
    [Tooltip("拖入前置 UI 上的'进入解密'按钮")]
    public Button enterButton;

    [Tooltip("拖入你的 puzzleCanvas（包含确认密码按钮）")]
    public CanvasGroup puzzleCanvas;

    [Header("解密控制与3D物体")]
    [Tooltip("★ 新增：拖入装有 3D 解密模块的父节点（比如你截图里的 contr' 或 puzzle）")]
    public GameObject puzzle3DObjects; // 控制 3D 物体的显示和隐藏

    [Tooltip("拖入挂载了你拖拽脚本(PuzzleManager)的物体")]
    public PuzzleManager puzzleManager;

    void Start()
    {
        SetPuzzleActive(false);
        // 4. 监听前置 UI 上的“进入”按钮
        if (enterButton != null)
        {
            enterButton.onClick.AddListener(OnEnterPuzzleClicked);
        }
    }

    // 当玩家点击“进入解密”按钮时执行
    void OnEnterPuzzleClicked()
    {
        // 1. 隐藏前置 UI
        if (preUICanvasGroup != null)
        {
            preUICanvasGroup.alpha = 0f;
            preUICanvasGroup.interactable = false;
            preUICanvasGroup.blocksRaycasts = false;
        }

        // 3. ★ 新增：让 3D 解密模块凭空出现！ // 4. 激活拖拽解密逻辑
        SetPuzzleActive(true);
    }
    public void SetPuzzleActive(bool isAct)
    {
        if (puzzle3DObjects != null)
        {
            puzzle3DObjects.SetActive(isAct);
        }
        if (puzzleManager != null)
        {
            puzzleManager.enabled = isAct;
            Debug.Log("解密正式开始，3D模块已出现，玩家可以拖拽了！");
        }
        if (puzzleCanvas != null)
        {
            puzzleCanvas.alpha = isAct ? 1f : 0f;
            puzzleCanvas.interactable = isAct;
            puzzleCanvas.blocksRaycasts = isAct;
        }
    }
}