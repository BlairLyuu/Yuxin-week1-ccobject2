using UnityEngine;
using System.Collections;

public class Action_BoatRide : MonoBehaviour
{
    public Transform player;
    public Transform boat;
    public Transform endPoint;
    public float speed = 5f;

    public void StartBoat()
    {
        StartCoroutine(BoatRoutine());
    }

    IEnumerator BoatRoutine()
    {
        // ★ 上锁
        GameManager.Instance.IsInteracting = true;
        
        // 1. 上船并定身
        GameManager.Instance.TogglePlayerControl(false);
        player.SetParent(boat);
        
        // 稍微修正一下玩家在船上的位置（可选）
        // player.localPosition = new Vector3(0, 1, 0); 

        // 2. 移动
        while (Vector3.Distance(boat.position, endPoint.position) > 0.1f)
        {
            boat.position = Vector3.MoveTowards(boat.position, endPoint.position, speed * Time.deltaTime);
            yield return null;
        }

        // 3. 下船
        player.SetParent(null);
        
        // ★ 解锁
        GameManager.Instance.TogglePlayerControl(true);
        GameManager.Instance.IsInteracting = false;
    }
}