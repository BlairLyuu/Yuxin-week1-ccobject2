using UnityEngine;

public class ElasticGlitchObject : MonoBehaviour
{
    [Header("拖拽设置")]
    public float maxDragTime = 3.0f; // 最多允许拖拽几秒
    public float returnSpeed = 5.0f; // 松手后飞回原位的速度

    [Header("轴向锁定 (打勾表示该轴被死死钉住)")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = true;  // 默认锁住Z轴(深度)，防止乱飘

    [Header("发光设置")]
    public Color glowColor = Color.cyan;
    public float glowIntensity = 3.0f;

    private Vector3 startPos;
    private float mZCoord;
    private Vector3 offset;

    private bool isDragging = false;
    private bool canDrag = true;
    private float currentDragTime = 0f;

    private Material myMaterial;
    private Color originalEmissionColor;

    void Start()
    {
        startPos = transform.position;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            myMaterial = rend.material;
            myMaterial.EnableKeyword("_EMISSION");
            if (myMaterial.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = myMaterial.GetColor("_EmissionColor");
            }
        }
    }

    void Update()
    {
        if (!isDragging || !canDrag)
        {
            // 平滑回缩
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * returnSpeed);
        }
    }

    void OnMouseDown()
    {
        if (!canDrag) return;

        isDragging = true;
        currentDragTime = 0f;

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseAsWorldPoint();

        SetGlow(true);
    }

    void OnMouseDrag()
    {
        if (!canDrag) return;

        currentDragTime += Time.deltaTime;

        if (currentDragTime >= maxDragTime)
        {
            canDrag = false;
            SetGlow(false);
            Debug.Log("物体挣脱了你的控制！");
            return;
        }

        // 1. 先计算出鼠标想让它去的位置
        Vector3 targetPos = GetMouseAsWorldPoint() + offset;

        // 2. 施加轴向锁定：如果某个轴被锁了，就强行把它按回初始的坐标
        if (lockX) targetPos.x = startPos.x;
        if (lockY) targetPos.y = startPos.y;
        if (lockZ) targetPos.z = startPos.z;

        // 3. 应用最终位置
        transform.position = targetPos;
    }

    void OnMouseUp()
    {
        isDragging = false;
        canDrag = true;
        SetGlow(false);
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void SetGlow(bool isGlowing)
    {
        if (myMaterial != null && myMaterial.HasProperty("_EmissionColor"))
        {
            if (isGlowing)
            {
                myMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
            }
            else
            {
                myMaterial.SetColor("_EmissionColor", originalEmissionColor);
            }
        }
    }
}