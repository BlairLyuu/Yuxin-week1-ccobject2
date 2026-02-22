using UnityEngine;
using UnityEngine.Events;

public class SlidingWall : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    // 新增：方向限制选项
    public enum DirectionLimit { Both, OnlyPositive, OnlyNegative }

    [Header("核心设置")]
    public Axis moveAxis = Axis.X;

    [Tooltip("允许拖拽的方向：两边都可以 / 只能正向 / 只能反向")]
    public DirectionLimit allowedDirection = DirectionLimit.Both;

    [Tooltip("能够拉开的最大距离")]
    public float maxMoveDistance = 3.0f;

    public float dragSpeed = 2.0f;
    public bool invertMouseInput = false;

    [Header("事件触发")]
    public UnityEvent onWallFullyOpened;

    private Vector3 startPos;
    private bool isLocked = false;

    void Start()
    {
        startPos = transform.position;
    }

    void OnMouseDrag()
    {
        if (isLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * dragSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * dragSpeed * Time.deltaTime;

        float directionMultiplier = invertMouseInput ? -1f : 1f;

        Vector3 newPos = transform.position;

        switch (moveAxis)
        {
            case Axis.X:
                newPos.x += mouseX * directionMultiplier;
                float minX = (allowedDirection == DirectionLimit.OnlyPositive) ? startPos.x : startPos.x - maxMoveDistance;
                float maxX = (allowedDirection == DirectionLimit.OnlyNegative) ? startPos.x : startPos.x + maxMoveDistance;
                newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                break;
            case Axis.Y:
                newPos.y += mouseY * directionMultiplier;
                float minY = (allowedDirection == DirectionLimit.OnlyPositive) ? startPos.y : startPos.y - maxMoveDistance;
                float maxY = (allowedDirection == DirectionLimit.OnlyNegative) ? startPos.y : startPos.y + maxMoveDistance;
                newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
                break;
            case Axis.Z:
                newPos.z += mouseY * directionMultiplier;
                float minZ = (allowedDirection == DirectionLimit.OnlyPositive) ? startPos.z : startPos.z - maxMoveDistance;
                float maxZ = (allowedDirection == DirectionLimit.OnlyNegative) ? startPos.z : startPos.z + maxMoveDistance;
                newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
                break;
        }

        transform.position = newPos;

        float currentDist = Vector3.Distance(transform.position, startPos);

        // 当拉到底时
        if (currentDist >= maxMoveDistance - 0.8f)
        {
            isLocked = true;
            Debug.Log("门完全拉开！开始倒计时 2 秒...");

            // 启动倒计时协程
            StartCoroutine(WaitAndTrigger());
        }
    }

    // 这是一个倒计时器
    private System.Collections.IEnumerator WaitAndTrigger()
    {
        yield return new WaitForSeconds(2.0f); // 等待2秒
        Debug.Log("2秒到了！触发切换！");
        onWallFullyOpened.Invoke(); // 触发事件
    }
}