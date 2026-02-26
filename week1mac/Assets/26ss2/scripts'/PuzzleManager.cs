using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    // ★ 修改点1：去掉了 [HideInInspector]，现在可以在 Unity 面板里直接拖拽赋值了
    [Header("解密方块与UI (必须在面板中赋值)")]
    public Transform block_1;
    public Transform block_2;
    public Transform block_3;
    public Button confirmButton;

    [Header("密码设置")]
    [SerializeField] BlockResult block1Password, block2Password, block3Password;
    private enum BlockResult
    {
        A, // 315
        B, // 45
        C, // 135
        D  // 225
    }
    private BlockResult block1Result, block2Result, block3Result;

    [Header("Settings")]
    public float dragSensitivity = 500f; // 鼠标拖动旋转灵敏度
    public float snapSpeed = 200f;       // 松手后自动归位旋转的速度

    private Transform currentSelectedBlock;
    // 使用字典跟踪每个方块当前正在运行的协程，以便随时打断
    private Dictionary<Transform, Coroutine> recoveryCoroutines = new Dictionary<Transform, Coroutine>();

    void Start()
    {
        // 绑定确认按钮的点击事件
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonPress);
        }
    }

    void Update()
    {
        HandleInput();
    }

    void OnConfirmButtonPress()
    {
        // 防错检查：确保方块都拖进去了
        if (block_1 == null || block_2 == null || block_3 == null)
        {
            Debug.LogError("⚠️ 方块没有赋值！请在 Inspector 面板中拖入 Block 1, 2, 3！");
            return;
        }

        // 1. 在判定前再次刷新三个方块的当前结果
        RecordBlockResult(block_1, block_1.localEulerAngles.x);
        RecordBlockResult(block_2, block_2.localEulerAngles.x);
        RecordBlockResult(block_3, block_3.localEulerAngles.x);

        // 2. Debug 打印当前方块的状态，方便调试
        Debug.Log($"当前状态: 方块1={block1Result}, 方块2={block2Result}, 方块3={block3Result}");

        // 3. 判断是否与设定的密码一致
        bool isCorrect = (block1Result == block1Password &&
                          block2Result == block2Password &&
                          block3Result == block3Password);

        if (isCorrect)
        {
            Debug.Log("<color=green>密码正确！谜题已解开。</color>");
        }
        else
        {
            Debug.Log("<color=red>密码错误，请继续尝试。</color>");
        }

        // ★ 修改点2：呼叫弹窗管理器弹出结果界面
        PuzzleResultManager resultManager = FindAnyObjectByType<PuzzleResultManager>();
        if (resultManager != null)
        {
            resultManager.ShowResult(isCorrect);
        }
        else
        {
            Debug.LogError("⚠️ 场景中找不到 PuzzleResultManager 脚本！请确保场景里有挂载它的物体。");
        }
    }

    // ==========================================
    // 下面完全保留你的原始拖拽和判断逻辑，未作任何修改
    // ==========================================

    private void HandleInput()
    {
        // 1. 鼠标按下：射线检测选中方块
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Transform hitTransform = hit.transform;
                if (hitTransform == block_1 || hitTransform == block_2 || hitTransform == block_3)
                {
                    currentSelectedBlock = hitTransform;

                    // 确保协程可被打断：如果该方块正在自动旋转恢复，立即停止该协程
                    if (recoveryCoroutines.ContainsKey(currentSelectedBlock) && recoveryCoroutines[currentSelectedBlock] != null)
                    {
                        StopCoroutine(recoveryCoroutines[currentSelectedBlock]);
                        recoveryCoroutines.Remove(currentSelectedBlock);
                    }
                }
            }
        }

        // 2. 鼠标按住：根据上下移动旋转方块
        if (Input.GetMouseButton(0) && currentSelectedBlock != null)
        {
            float mouseY = Input.GetAxis("Mouse Y");
            currentSelectedBlock.Rotate(Vector3.right, mouseY * dragSensitivity, Space.Self);
        }

        // 3. 鼠标松开：启动恢复协程
        if (Input.GetMouseButtonUp(0) && currentSelectedBlock != null)
        {
            Coroutine snapCoroutine = StartCoroutine(SnapBlockCoroutine(currentSelectedBlock));
            recoveryCoroutines[currentSelectedBlock] = snapCoroutine;
            currentSelectedBlock = null;
        }
    }

    private IEnumerator SnapBlockCoroutine(Transform block)
    {
        float currentX = block.localEulerAngles.x;
        float[] targetAngles = { 45f, 135f, 225f, 315f };
        float closestTarget = targetAngles[0];
        float minDiff = Mathf.Abs(Mathf.DeltaAngle(currentX, closestTarget));

        foreach (float target in targetAngles)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(currentX, target));
            if (diff < minDiff)
            {
                minDiff = diff;
                closestTarget = target;
            }
        }

        while (Mathf.Abs(Mathf.DeltaAngle(block.localEulerAngles.x, closestTarget)) > 0.05f)
        {
            float newX = Mathf.MoveTowardsAngle(block.localEulerAngles.x, closestTarget, snapSpeed * Time.deltaTime);
            Vector3 euler = block.localEulerAngles;
            euler.x = newX;
            block.localEulerAngles = euler;
            yield return null;
        }

        Vector3 finalEuler = block.localEulerAngles;
        finalEuler.x = closestTarget;
        block.localEulerAngles = finalEuler;
    }

    private void RecordBlockResult(Transform block, float dummyAngle)
    {
        Vector3 localUp = block.localRotation * Vector3.up;
        float signedAngle = Vector3.SignedAngle(Vector3.up, localUp, Vector3.right);
        float angle = Mathf.Repeat(signedAngle, 360f);

        BlockResult res = BlockResult.A;

        if (Mathf.Abs(angle - 45f) < 5f) res = BlockResult.B;
        else if (Mathf.Abs(angle - 135f) < 5f) res = BlockResult.C;
        else if (Mathf.Abs(angle - 225f) < 5f) res = BlockResult.D;
        else if (Mathf.Abs(angle - 315f) < 5f) res = BlockResult.A;

        if (block == block_1) block1Result = res;
        else if (block == block_2) block2Result = res;
        else if (block == block_3) block3Result = res;
    }
}