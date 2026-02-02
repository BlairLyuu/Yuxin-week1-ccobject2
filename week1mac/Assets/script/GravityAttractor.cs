using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f; // 重力大小

    public void Attract(Transform body)
    {
        // 1. 发射射线寻找脚下的“地面”
        // 我们从玩家的脚部稍微向上一点的位置发射，防止射线起点在地面里面
        Vector3 rayOrigin = body.position + body.up * 0.5f; 
        RaycastHit hit;
        
        Vector3 gravityUp = Vector3.zero;

        // 向下探测 (注意：是相对于玩家当前的“下”，不是世界坐标的下)
        if (Physics.Raycast(rayOrigin, -body.up, out hit, 5f)) 
        {
            // 找到了地面，重力方向就是地面的法线方向
            gravityUp = hit.normal;
        }
        else 
        {
            // 如果悬空了（没踩到面），就默认吸向正方体的中心
            // 这能防止你在棱角跳起来时飞出太空
            gravityUp = (body.position - transform.position).normalized;
        }

        Vector3 bodyUp = body.up;

        // 2. 施加物理重力 (让刚体往下掉)
        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        // 3. 旋转玩家 (最关键的一步)
        // 计算目标旋转：让玩家的头顶 (bodyUp) 对齐 地面法线 (gravityUp)
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        
        // 平滑插值旋转 (Slerp)，让过渡顺滑，不会在过棱角时瞬间抖动
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50f * Time.deltaTime);
    }
}