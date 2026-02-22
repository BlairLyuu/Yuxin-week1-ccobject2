using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用这个才能切换场景

public class SceneLoader : MonoBehaviour
{
    [Tooltip("填入你下一个场景的名字，比如 MirrorRoom")]
    public string nextSceneName;

    // 这个方法会被 UI 按钮调用
    public void LoadNextLevel()
    {
        Debug.Log("准备进入: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}