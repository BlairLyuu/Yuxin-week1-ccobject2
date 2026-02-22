using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragUpToTransition : MonoBehaviour
{
    [Header("交互开关")]
    public bool canDrag = false; // 默认锁死，不能拖

    [Header("拖拽设置")]
    public float dragDistance = 3.0f;
    public float dragSpeed = 2.0f;

    [Header("转场设置")]
    public CanvasGroup blackScreen;
    public float fadeSpeed = 1.5f;
    public string nextSceneName = "MirrorRoom";

    private float startY;
    private bool isTriggered = false;

    void Start()
    {
        startY = transform.position.y;
    }

    // 这是留给 ENTER 按钮调用的开锁方法！
    public void UnlockDragging()
    {
        canDrag = true;
        Debug.Log("封印解除，可以拖拽了！");
    }

    void OnMouseDrag()
    {
        // 如果没解锁，或者已经黑屏了，直接无视鼠标
        if (!canDrag || isTriggered) return;

        float mouseY = Input.GetAxis("Mouse Y") * dragSpeed * Time.deltaTime;

        Vector3 newPos = transform.position + new Vector3(0, mouseY, 0);
        newPos.y = Mathf.Clamp(newPos.y, startY, startY + dragDistance);
        transform.position = newPos;

        if (transform.position.y >= startY + dragDistance - 0.1f)
        {
            isTriggered = true;
            Debug.Log("拉到底了！开始执行黑屏转场...");
            StartCoroutine(FadeToBlackAndLoad());
        }
    }

    private IEnumerator FadeToBlackAndLoad()
    {
        if (blackScreen != null)
        {
            // 确保黑屏物体是激活状态
            blackScreen.gameObject.SetActive(true);

            while (blackScreen.alpha < 1)
            {
                blackScreen.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
        else
        {
            Debug.LogError("警告：你没有把黑屏UI拖进脚本的槽位里！");
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextSceneName);
    }
}